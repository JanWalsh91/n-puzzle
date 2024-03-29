﻿using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.FB.Demo
{
    /// <summary>Examples for all methods.</summary>
    [HelpURL("https://www.crosstales.com/media/data/assets/FileBrowser/api/class_crosstales_1_1_f_b_1_1_demo_1_1_examples.html")]
    public class Examples : MonoBehaviour
    {
        #region Variables

        public GameObject TextPrefab;

        public GameObject ScrollView;
        
        public Text Error;
         
        #endregion


        #region Public methods

        public void OpenSingleFile() {
            //Debug.Log("OpenSingleFile");
            
            /*
            ExtensionFilter[] extensions = new[] {
                new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
                new ExtensionFilter("Sound Files", "mp3", "wav" ),
                new ExtensionFilter("All Files", "*" ),
            };
            */
            
            string extensions = string.Empty;
            
            string path = FileBrowser.OpenSingleFile("Open File", string.Empty, extensions);

            //Debug.Log("Selected file: " + path);
            
            rebuildList(path);
        }
        
        public void OpenFiles() {
            //Debug.Log("OpenFiles");
            
            /*
            ExtensionFilter[] extensions = new[] {
                new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
                new ExtensionFilter("Sound Files", "mp3", "wav" ),
                new ExtensionFilter("All Files", "*" ),
            };
            */
            
            string extensions = string.Empty;
            
            string[] paths = FileBrowser.OpenFiles("Open Files", string.Empty, extensions, true);

            /*
            foreach (string path in paths)
            {
                Debug.Log("Selected file: " + path);
            }
            */
            
            rebuildList(paths);
        }
        
        public void OpenSingleFolder() {
            //Debug.Log("OpenSingleFolder");
            
            string path = FileBrowser.OpenSingleFolder("Open Folder");

            //Debug.Log("Selected folder: " + path);

            //Debug.Log("Files: " + FileBrowser.GetFiles(path, "*").CTDump());
            //Debug.Log("Directories: " + FileBrowser.GetDirectories(path).CTDump());

            rebuildList(path);
        }
        
        public void OpenFolders() {
            //Debug.Log("OpenFolders");
            
            //string[] paths = FileBrowser.OpenFolders("Open Files", string.Empty, true);
            string[] paths = FileBrowser.OpenFolders("Open Folders");

            /*
            foreach (string path in paths)
            {
                Debug.Log("Selected folder: " + path);
            }
            */
            
            rebuildList(paths);
        }
        
        public void SaveFile() {
            //Debug.Log("SaveFile");
            
            /*
            ExtensionFilter[] extensions = new[] {
                        new ExtensionFilter("Binary", "bin"),
                        new ExtensionFilter("Text", "txt"),
                        new ExtensionFilter("C#", "cs"),
                    };
            */
            
            string extensions = "txt";
            
            string path = FileBrowser.SaveFile("Save File", string.Empty, "MySaveFile", extensions);
            
            //Debug.Log("Save file: " + path);
            
            rebuildList(path);
        }

        public void OpenFilesAsync() {
            //Debug.Log("OpenFilesAsync");
            
            /*
            ExtensionFilter[] extensions = new[] {
                new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
                new ExtensionFilter("Sound Files", "mp3", "wav" ),
                new ExtensionFilter("All Files", "*" ),
            };
            */
            
            string extensions = string.Empty;
            
            FileBrowser.OpenFilesAsync("Open Files", string.Empty, extensions, true, (string[] paths) => { writePaths(paths); });
        }
        
        public void OpenFoldersAsync() {
            //Debug.Log("OpenFoldersAsync");
            
            FileBrowser.OpenFoldersAsync("Open Folders", string.Empty, true, (string[] paths) => { writePaths(paths); });
        }
        
        public void SaveFileAsync() {
            //Debug.Log("SaveFileAsync");
            
            /*
            ExtensionFilter[] extensions = new[] {
                        new ExtensionFilter("Binary", "bin"),
                        new ExtensionFilter("Text", "txt"),
                        new ExtensionFilter("C#", "cs"),
                    };
            */
            
            string extensions = "txt";
            
            FileBrowser.SaveFileAsync("Save File", string.Empty, "MySaveFile", extensions, (string paths) => { writePaths(paths); });
        }
        
        private void writePaths(params string[] paths) {
            /*
            foreach (string path in paths)
            {
                Debug.Log("Selected path: " + path);
            }
            */
            
            rebuildList(paths);
        }

        #endregion

        private void rebuildList(params string[] e)
        {
            for (int ii = ScrollView.transform.childCount - 1; ii >= 0; ii--)
            {
                Transform child = ScrollView.transform.GetChild(ii);
                child.SetParent(null);
                Destroy(child.gameObject);
            }

            ScrollView.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 80 * e.Length);

            for (int ii = 0; ii < e.Length; ii++)
            {
                //if (Config.DEBUG)
                //    Debug.Log(e[ii]);

                GameObject go = Instantiate(TextPrefab);

                go.transform.SetParent(ScrollView.transform);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = new Vector3(10, -80 * ii, 0);
                go.GetComponent<Text>().text = e[ii].ToString();
            }
        }
    }
}
// © 2017-2019 crosstales LLC (https://www.crosstales.com)