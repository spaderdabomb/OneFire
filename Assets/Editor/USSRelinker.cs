using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace USSRelinker
{
    public class USSAutoRelinkerWindow : EditorWindow
    {
        [System.Serializable]
        public class BrokenLink
        {
            public string uxmlPath;
            public string originalUssPath;
            public string originalUri;
            public string fileName;
            public bool canFix;
            public string newPath;
            public string newUri;
            public bool linkFixed;
        }

        private Vector2 scrollPosition;
        private List<BrokenLink> brokenLinks = new List<BrokenLink>();
        private List<string> uxmlFiles = new List<string>();
        private List<string> ussFiles = new List<string>();
        private bool isScanning = false;
        private string scanStatus = "";
        private bool showFixedOnly = false;
        private bool showUnfixableOnly = false;
        private string searchFilter = "";

        [MenuItem("Tools/USS Auto-Relinker")]
        public static void ShowWindow()
        {
            GetWindow<USSAutoRelinkerWindow>("USS Auto-Relinker");
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("USS Auto-Relinker", EditorGUIUtility.IconContent("d_ScriptableObject Icon").image);
            minSize = new Vector2(600, 400);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // Header
            GUILayout.Space(10);
            EditorGUILayout.LabelField("USS File Auto-Relinker", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Automatically fix broken USS references in UXML files", EditorStyles.miniLabel);
            GUILayout.Space(10);

            // Scan section
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = !isScanning;
            if (GUILayout.Button("Scan Project for Broken USS Links", GUILayout.Height(30)))
            {
                ScanProject();
            }
            GUI.enabled = true;
            
            if (GUILayout.Button("Refresh Asset Database", GUILayout.Width(150), GUILayout.Height(30)))
            {
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Refresh Complete", "Asset database has been refreshed.", "OK");
            }
            EditorGUILayout.EndHorizontal();

            if (isScanning)
            {
                EditorGUILayout.LabelField(scanStatus);
                EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), 0.5f, "Scanning...");
            }

            GUILayout.Space(10);

            // Statistics
            if (brokenLinks.Count > 0)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                int fixableCount = brokenLinks.Count(link => link.canFix);
                int fixedCount = brokenLinks.Count(link => link.linkFixed);
                
                EditorGUILayout.LabelField($"Total Issues: {brokenLinks.Count}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Fixable: {fixableCount}", fixableCount > 0 ? EditorStyles.boldLabel : EditorStyles.label);
                EditorGUILayout.LabelField($"Fixed: {fixedCount}", fixedCount > 0 ? EditorStyles.boldLabel : EditorStyles.label);
                EditorGUILayout.EndHorizontal();

                // Filter options
                EditorGUILayout.BeginHorizontal();
                searchFilter = EditorGUILayout.TextField("Search Filter:", searchFilter);
                showFixedOnly = EditorGUILayout.Toggle("Show Fixed Only", showFixedOnly, GUILayout.Width(120));
                showUnfixableOnly = EditorGUILayout.Toggle("Show Unfixable Only", showUnfixableOnly, GUILayout.Width(150));
                EditorGUILayout.EndHorizontal();

                // Fix all button
                EditorGUILayout.BeginHorizontal();
                GUI.enabled = fixableCount > 0 && !isScanning;
                if (GUILayout.Button("Fix All Broken Links", GUILayout.Height(25)))
                {
                    FixAllLinks();
                }
                GUI.enabled = true;

                if (fixedCount > 0)
                {
                    if (GUILayout.Button("Apply All Fixes to Files", GUILayout.Height(25)))
                    {
                        ApplyAllFixes();
                    }
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10);

                // Results list
                DrawBrokenLinksList();
            }
            else if (!isScanning)
            {
                EditorGUILayout.LabelField("No broken USS links found. Click 'Scan Project' to check for issues.", EditorStyles.centeredGreyMiniLabel);
            }

            EditorGUILayout.EndVertical();
        }

        private void ScanProject()
        {
            isScanning = true;
            scanStatus = "Scanning project...";
            brokenLinks.Clear();

            try
            {
                // Find all UXML and USS files
                FindProjectFiles();
                
                scanStatus = $"Found {uxmlFiles.Count} UXML files and {ussFiles.Count} USS files";
                
                // Scan each UXML file for broken USS links
                for (int i = 0; i < uxmlFiles.Count; i++)
                {
                    string uxmlPath = uxmlFiles[i];
                    scanStatus = $"Scanning {Path.GetFileName(uxmlPath)} ({i + 1}/{uxmlFiles.Count})";
                    
                    ScanUXMLFile(uxmlPath);
                    
                    // Update progress
                    if (EditorUtility.DisplayCancelableProgressBar("Scanning USS Links", 
                        scanStatus, (float)i / uxmlFiles.Count))
                    {
                        break;
                    }
                }

                // Try to find fixes for broken links
                scanStatus = "Finding potential fixes...";
                FindPotentialFixes();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                isScanning = false;
                scanStatus = $"Scan complete. Found {brokenLinks.Count} broken USS references.";
                Repaint();
            }
        }

        private void FindProjectFiles()
        {
            uxmlFiles.Clear();
            ussFiles.Clear();

            string[] allAssets = AssetDatabase.GetAllAssetPaths();
            
            foreach (string assetPath in allAssets)
            {
                if (assetPath.EndsWith(".uxml", StringComparison.OrdinalIgnoreCase))
                {
                    uxmlFiles.Add(assetPath);
                }
                else if (assetPath.EndsWith(".uss", StringComparison.OrdinalIgnoreCase))
                {
                    ussFiles.Add(assetPath);
                }
            }
        }

        private void ScanUXMLFile(string uxmlPath)
        {
            try
            {
                string content = File.ReadAllText(uxmlPath);
                
                // Regex to find USS references in UXML files
                Regex uriRegex = new Regex(@"project://database/(Assets/[^""]*\.uss)\?[^""]*", RegexOptions.IgnoreCase);
                
                MatchCollection matches = uriRegex.Matches(content);
                
                foreach (Match match in matches)
                {
                    string fullUri = match.Value;
                    string ussPath = match.Groups[1].Value;
                    string fileName = Path.GetFileName(ussPath);
                    
                    // Check if the USS file exists at the specified path
                    if (!File.Exists(ussPath))
                    {
                        BrokenLink brokenLink = new BrokenLink
                        {
                            uxmlPath = uxmlPath,
                            originalUssPath = ussPath,
                            originalUri = fullUri,
                            fileName = fileName,
                            canFix = false,
                            linkFixed = false
                        };
                        
                        brokenLinks.Add(brokenLink);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error scanning UXML file {uxmlPath}: {e.Message}");
            }
        }

        private void FindPotentialFixes()
        {
            foreach (BrokenLink brokenLink in brokenLinks)
            {
                // Try to find USS file with the same name
                List<string> candidates = ussFiles.Where(path => 
                    Path.GetFileName(path).Equals(brokenLink.fileName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (candidates.Count == 1)
                {
                    // Exact match found
                    brokenLink.canFix = true;
                    brokenLink.newPath = candidates[0];
                }
                else if (candidates.Count > 1)
                {
                    // Multiple candidates - try to find the best match
                    string bestMatch = FindBestMatch(brokenLink.originalUssPath, candidates);
                    if (bestMatch != null)
                    {
                        brokenLink.canFix = true;
                        brokenLink.newPath = bestMatch;
                    }
                }
            }
        }

        private string FindBestMatch(string originalPath, List<string> candidates)
        {
            // Simple heuristic: prefer paths that share more directory components
            string[] originalParts = originalPath.Split('/');
            
            int bestScore = 0;
            string bestMatch = null;
            
            foreach (string candidate in candidates)
            {
                string[] candidateParts = candidate.Split('/');
                int score = 0;
                
                // Count matching directory components
                for (int i = 0; i < Math.Min(originalParts.Length - 1, candidateParts.Length - 1); i++)
                {
                    if (originalParts[i].Equals(candidateParts[i], StringComparison.OrdinalIgnoreCase))
                    {
                        score++;
                    }
                }
                
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMatch = candidate;
                }
            }
            
            return bestMatch;
        }

        private void FixAllLinks()
        {
            foreach (BrokenLink link in brokenLinks.Where(l => l.canFix && !l.linkFixed))
            {
                FixLink(link);
            }
            Repaint();
        }

        private void FixLink(BrokenLink link)
        {
            if (!link.canFix || string.IsNullOrEmpty(link.newPath))
                return;

            // Generate new URI with correct GUID and fileID
            string guid = AssetDatabase.AssetPathToGUID(link.newPath);
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogError($"Could not get GUID for {link.newPath}");
                return;
            }

            // For USS files, we typically use fileID=7433441132597879392 and type=3
            link.newUri = $"project://database/{link.newPath}?fileID=7433441132597879392&amp;guid={guid}&amp;type=3#{Path.GetFileNameWithoutExtension(link.newPath)}";
            link.linkFixed = true;
        }

        private void ApplyAllFixes()
        {
            var groupedFixes = brokenLinks.Where(l => l.linkFixed)
                .GroupBy(l => l.uxmlPath)
                .ToList();

            foreach (var group in groupedFixes)
            {
                ApplyFixesToFile(group.Key, group.ToList());
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Fixes Applied", 
                $"Applied fixes to {groupedFixes.Count} UXML files.", "OK");
        }

        private void ApplyFixesToFile(string uxmlPath, List<BrokenLink> fixes)
        {
            try
            {
                string content = File.ReadAllText(uxmlPath);
                
                foreach (BrokenLink fix in fixes)
                {
                    content = content.Replace(fix.originalUri, fix.newUri);
                }
                
                File.WriteAllText(uxmlPath, content);
                Debug.Log($"Applied {fixes.Count} fixes to {uxmlPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error applying fixes to {uxmlPath}: {e.Message}");
            }
        }

        private void DrawBrokenLinksList()
        {
            var filteredLinks = brokenLinks.AsEnumerable();

            // Apply filters
            if (showFixedOnly)
                filteredLinks = filteredLinks.Where(l => l.linkFixed);
            else if (showUnfixableOnly)
                filteredLinks = filteredLinks.Where(l => !l.canFix);

            if (!string.IsNullOrEmpty(searchFilter))
            {
                filteredLinks = filteredLinks.Where(l => 
                    l.uxmlPath.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    l.fileName.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            var linksList = filteredLinks.ToList();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (BrokenLink link in linksList)
            {
                DrawBrokenLink(link);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawBrokenLink(BrokenLink link)
        {
            Color originalColor = GUI.backgroundColor;
            
            if (link.linkFixed)
                GUI.backgroundColor = Color.green;
            else if (link.canFix)
                GUI.backgroundColor = Color.yellow;
            else
                GUI.backgroundColor = Color.red;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.backgroundColor = originalColor;

            // Header
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"UXML: {Path.GetFileName(link.uxmlPath)}", EditorStyles.boldLabel);
            
            if (link.linkFixed)
                EditorGUILayout.LabelField("✓ FIXED", EditorStyles.boldLabel, GUILayout.Width(60));
            else if (link.canFix)
                EditorGUILayout.LabelField("CAN FIX", EditorStyles.label, GUILayout.Width(60));
            else
                EditorGUILayout.LabelField("NO FIX", EditorStyles.label, GUILayout.Width(60));

            EditorGUILayout.EndHorizontal();

            // Details
            EditorGUILayout.LabelField($"Missing USS: {link.fileName}", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Original Path: {link.originalUssPath}", EditorStyles.miniLabel);
            
            if (link.canFix)
            {
                EditorGUILayout.LabelField($"New Path: {link.newPath}", EditorStyles.miniLabel);
            }

            // Actions
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Select UXML", GUILayout.Width(100)))
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(link.uxmlPath);
                EditorGUIUtility.PingObject(Selection.activeObject);
            }

            if (link.canFix && !link.linkFixed)
            {
                if (GUILayout.Button("Fix This Link", GUILayout.Width(100)))
                {
                    FixLink(link);
                }
            }

            if (link.linkFixed)
            {
                if (GUILayout.Button("Apply Fix", GUILayout.Width(100)))
                {
                    ApplyFixesToFile(link.uxmlPath, new List<BrokenLink> { link });
                    AssetDatabase.Refresh();
                }
            }

            if (link.canFix && !string.IsNullOrEmpty(link.newPath))
            {
                if (GUILayout.Button("Select USS", GUILayout.Width(100)))
                {
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(link.newPath);
                    EditorGUIUtility.PingObject(Selection.activeObject);
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
        }
    }
}