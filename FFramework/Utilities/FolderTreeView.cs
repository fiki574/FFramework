/*
    C# Framework with a lot of useful functions and classes
    Copyright (C) 2016 Bruno Fištrek
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
    
    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.IO;
using System.Windows.Controls;

namespace FFramework.Utilities
{
    public class FolderTreeView
    {
        private string path;
        private string folder;

        public FolderTreeView(string folder)
        {
            this.folder = folder;
            path = null;
        }

        public void CreateTree(DirectoryInfo folder, TreeViewItem parentNode)
        {
            DirectoryInfo[] dirs = folder.GetDirectories();
            if (dirs.Length > 0)
            {
                foreach (DirectoryInfo dir in dirs)
                {
                    TreeViewItem newi = new TreeViewItem();
                    newi.Header = dir.Name;
                    FileInfo[] files = dir.GetFiles();
                    foreach (FileInfo file in files)
                        newi.Items.Add(new TreeViewItem() { Header = file.Name });
                    parentNode.Items.Add(newi);
                    CreateTree(dir, newi);
                }
            }
        }

        public bool IsItemFolder(TreeViewItem item)
        {
            return item.Items.Count == 0 ? false : true;
        }

        public string GetPath(TreeViewItem item)
        {
            path = null;
            FilePath(item);
            return ReversePath();
        }

        private void FilePath(TreeViewItem item)
        {
            TreeViewItem parent = item.Parent as TreeViewItem;
            if (parent != null)
            {
                path += "\\" + item.Header;
                FilePath(parent);
            }
        }

        private string ReversePath()
        {
            if (path != null)
            {
                string[] p = path.Split('\\');
                string actual_path = "";
                for (int i = p.Length - 1; i > 0; i--)
                    actual_path += p[i] + "\\";
                return folder + "\\" + actual_path.Remove(actual_path.Length - 1);
            }
            else
                return null;
        }
    }
}
