﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using KSPModAdmin.Core.Controller;
using KSPModAdmin.Core.Model;
using KSPModAdmin.Core.Utils.Controls.Aga.Controls.Tree;
using SharpCompress.Archive;

namespace KSPModAdmin.Core.Utils
{
    public static class ModNodeHandler
    {
        #region Constants

        const string TYPE = "type = ";

        #endregion

        #region Static Member variables

        /// <summary>
        /// Holds all not processed directories to delete.
        /// </summary>
        private static List<ModNode> mNotDeletedDirs = new List<ModNode>();

        #endregion

        #region ModNode creation

        /// <summary>
        /// Creates a tree of TreeNodeMod nodes that represent the content of a mod archive.
        /// </summary>
        /// <param name="modInfo">The ModInfo of the mod the create a tree for.</param>
        /// <param name="silent">Determines if info messages should be added.</param>
        /// <returns>A tree of TreeNodeMod nodes that represent the content of a mod archive.</returns>
        public static ModNode CreateModNode(ModInfo modInfo, bool silent = false)
        {
            if (File.Exists(modInfo.LocalPath))
            {
                ModNode node = new ModNode(modInfo);
                using (IArchive archive = ArchiveFactory.Open(modInfo.LocalPath))
                {
                    char seperator = '/';
                    string extension = Path.GetExtension(modInfo.LocalPath);
                    if (extension != null && extension.ToLower() == Constants.EXT_RAR)
                        seperator = '\\';

                    // create a TreeNode for every archive entry
                    foreach (IArchiveEntry entry in archive.Entries)
                        CreateModNode(entry.FilePath, node, seperator, entry.IsDirectory, silent);

                }

                // Find installation root node (first folder that contains (Parts or Plugins or ...)
                if (!FindAndSetDestinationPaths(node) && !silent)
                    Messenger.AddInfo(string.Format(Messages.MSG_ROOT_NOT_FOUND_0, node.Text));

                SetToolTips(node);
                CheckNodesWithDestination(node);

                return node;
            }
            else
            {
                if (!silent)
                    Messenger.AddInfo(string.Format(Messages.MSG_MOD_ZIP_NOT_FOUND_0, modInfo.LocalPath));
            }

            return null;
        }

        /// <summary>
        /// Creates a TreeNode.
        /// </summary>
        /// <param name="filename">Zip-File path</param>
        /// <param name="parent">The parent node where the created node will be attached attach to.</param>
        /// <param name="pathSeperator">The seperator charater used within the filename.</param>
        /// <param name="silent">Determines if info messages should be added.</param>
        private static void CreateModNode(string filename, ModNode parent, char pathSeperator, bool isDirectory, bool silent = false)
        {
            // ignore directory entries.
            if (!isDirectory)
                HandleFileEntry(filename, parent, pathSeperator, silent);
        }

        /// <summary>
        /// Handles and creates a file entry.
        /// </summary>
        /// <param name="filename">Zip-File path</param>
        /// <param name="parent">The parent node where the created node will be attached attach to.</param>
        /// <param name="pathSeperator">The seperator charater used within the filename.</param>
        /// <param name="silent">Determines if info messages should be added.</param>
        private static void HandleFileEntry(string filename, ModNode parent, char pathSeperator, bool silent = false)
        {
            // plain filename?
            if (!filename.Contains(pathSeperator))
            {
                CreateFileListEntry(filename, parent, silent);
            }
            else // filename with dir(s) infront
            {
                ModNode node = CreateNeededDirNodes(filename, parent, pathSeperator);
                CreateFileListEntry(filename, node, silent);
            }
        }

        /// <summary>
        /// Splits the filename at the pathSeperator and creates a dir node for each split part.
        /// </summary>
        /// <param name="filename">Fullpath within the archive.</param>
        /// <param name="parent">The parent TreeNode.</param>
        /// <param name="pathSeperator">The path seperator that is used within the archive.</param>
        /// <returns>The last created node.</returns>
        private static ModNode CreateNeededDirNodes(string filename, ModNode parent, char pathSeperator)
        {
            ModNode dirNode = parent;
            string[] dirs = filename.Split(pathSeperator);
            for (int i = 0; i < dirs.Length - 1; ++i)
            {
                if (dirNode.ContainsChild(dirs[i]))
                    dirNode = dirNode[dirs[i]];
                else
                    dirNode = CreateDirListEntry(dirs[i], dirNode);
            }

            return dirNode;
        }

        /// <summary>
        /// Creates a file entry for the TreeView. 
        /// </summary>
        /// <param name="fileName">Zip-File path of the file.</param>
        /// <param name="parent">The parent node where the created node will be attached attach to.</param>
        /// <param name="silent">Determines if info messages should be added.</param>
        private static void CreateFileListEntry(string fileName, ModNode parent, bool silent = false)
        {
            ModNode node = new ModNode(fileName, Path.GetFileName(fileName), NodeType.UnknownFile);
            // TODO:!!!
            //node.ToolTipText = "<No path selected>";
            parent.Nodes.Add(node);

            if (!silent)
                Messenger.AddInfo(string.Format(Messages.MSG_FILE_ADDED_0, fileName));
        }

        /// <summary>
        /// Creates a directory entry for the TreeView. 
        /// </summary>
        /// <param name="dirName">Name of the directory.</param>
        /// <param name="parent">The parent node where the created node will be attached attach to.</param>
        private static ModNode CreateDirListEntry(string dirName, ModNode parent)
        {
            // dir already created?
            ModNode dirNode = ModSelectionTreeModel.SearchNodeByPath(parent.Text + "/" + dirName, parent, '/');
            if (null == dirNode)
            {
                dirNode = new ModNode(dirName, dirName);
                // TODO:!!!
                //dirNode.ToolTipText = "<No path selected>";
                dirNode.NodeType = (KSPPathHelper.IsKSPDir(dirName.ToLower())) ? NodeType.KSPFolder : NodeType.UnknownFolder;
                parent.Nodes.Add(dirNode);

                Messenger.AddInfo(string.Format(Messages.MSG_DIR_ADDED_0, dirName));
            }

            return dirNode;
        }

        #endregion

        #region Copy mod

        /// <summary>
        /// Tries to find notes in the new mod, that matches to the outdated mod.
        /// If a matching node was found the destination and/or the checked state of the node will be copied.
        /// </summary>
        /// <param name="outdatedMod">The outdated mod.</param>
        /// <param name="newMod">The new (updated) mod.</param>
        /// <returns>True if matching files where found, otherwise false.</returns>
        public static bool TryCopyDestToMatchingNodes(ModNode outdatedMod, ModNode newMod)
        {
            //if (outdatedMod.Name == newMod.Name)
            //    return true;

            bool matchFound = false;
            List<ModNode> outdatedFileNodes = outdatedMod.GetAllFileNodes();
            if (outdatedFileNodes.Count == 0)
                return matchFound;

            //List<Tuple<TreeNodeMod, Tuple<TreeNodeMod, TreeNodeMod>>> newMatchingFileNodes1 = 
            //    new List<Tuple<TreeNodeMod, Tuple<TreeNodeMod, TreeNodeMod>>>();
            foreach (var node in outdatedFileNodes)
            {
                ModNode parentOld = (ModNode)node.Parent;
                if (parentOld == null)
                    continue;

                string path = parentOld.Text + '\\' + node.Text;
                ModNode matchingNew = ModSelectionTreeModel.SearchNodeByPath(path, newMod, '\\');
                if (matchingNew == null)
                    continue;

                matchFound = true;
                if (OptionsController.ModUpdateBehavior == ModUpdateBehavior.CopyDestination)
                {
                    matchingNew.Destination = node.Destination;
                    ((ModNode)matchingNew.Parent).Destination = ((ModNode)node.Parent).Destination;
                }
                matchingNew.Checked = node.Checked;
                ((ModNode)matchingNew.Parent).Checked = ((ModNode)node.Parent).Checked;

                ModNode parentNew = matchingNew;
                while (parentOld != null)
                {
                    if (parentOld.Parent == null)
                        break;

                    path = parentOld.Parent.Text + '\\' + path;
                    if (ModSelectionTreeModel.SearchNodeByPath(path, newMod, '\\') == null)
                        break;

                    parentNew = (ModNode)parentNew.Parent;
                    if (parentNew == null)
                        break;

                    if (OptionsController.ModUpdateBehavior == ModUpdateBehavior.CopyDestination)
                        parentNew.Destination = parentOld.Destination;
                    parentNew.Checked = parentOld.Checked;
                    parentOld = (ModNode)parentOld.Parent;
                }

                //newMatchingFileNodes1.Add(new Tuple<TreeNodeMod, Tuple<TreeNodeMod, TreeNodeMod>>(parentOld,
                //    new Tuple<TreeNodeMod, TreeNodeMod>(matchingNew, node)));
            }

            return matchFound;
        }

        #endregion

        #region NodeChecking

        /// <summary>
        /// Checks the node and all child nodes that have a destination.
        /// </summary>
        /// <param name="node">The node to check.</param>
        public static void CheckNodesWithDestination(ModNode node)
        {
            if (node.HasDestination)
                CheckNodeAndParents(node);

            foreach (ModNode child in node.Nodes)
                CheckNodesWithDestination(child);
        }

        /// <summary>
        /// Checks the node and all parents.
        /// </summary>
        /// <param name="node">The node to check.</param>
        private static void CheckNodeAndParents(ModNode node)
        {
            // check node and parent nodes.
            node.SetChecked(true);

            if (node.Parent == null || node.Parent.Index == -1)
                return;

            Node parent = node.Parent;
            while (parent != null && parent.Index > -1)
            {
                ((ModNode)parent).SetChecked(true);
                parent = parent.Parent;
            }
        }

        #endregion

        #region Set destination paths

        /// <summary>
        /// Finds the root folder of the mod that can be installed to the KSP install folder.
        /// </summary>
        /// <param name="node">Node to start the search from.</param>
        /// <returns>The root folder of the mod that can be installed to the KSP install folder.</returns>
        private static bool FindAndSetDestinationPaths(ModNode node)
        {
            List<ModNode> kspFolders = new List<ModNode>();
            List<ModNode> craftFiles = new List<ModNode>();
            ModSelectionTreeModel.GetAllKSPFolders(node, ref kspFolders, ref craftFiles);
            if (kspFolders.Count == 1)
            {
                SetDestinationPaths(kspFolders[0], false);
            }
            else if (kspFolders.Count > 1)
            {
                kspFolders.Sort((node1, node2) =>
                {
                    if (node2.Depth == node1.Depth)
                        return node1.Text.CompareTo(node2.Text);
                    else
                        return node2.Depth - node1.Depth;
                });

                bool lastResult = false;
                foreach (ModNode kspFolder in kspFolders)
                    lastResult = SetDestinationPaths(kspFolder, lastResult);
            }

            if (craftFiles.Count > 0)
            {
                foreach (ModNode craftNode in craftFiles)
                {
                    string vab = KSPPathHelper.GetPath(KSPPaths.VAB);
                    string sph = KSPPathHelper.GetPath(KSPPaths.SPH);
                    if (!craftNode.HasDestination || (!craftNode.Destination.StartsWith(vab) && !craftNode.Destination.StartsWith(sph)))
                        SetCraftDestination(craftNode);
                }
            }

            if (node.HasDestination || node.HasDestinationForChilds)
                node.SetChecked(true);

            return (kspFolders.Count > 0) || (craftFiles.Count > 0);
        }


        /// <summary>
        /// Builds and sets the destination path to the passed node and its childes.
        /// </summary>
        /// <param name="node">Node to set the destination path.</param>
        /// <param name="gameDataFound">Flag to inform the function it the GameData folder was already found (for calls from a loop).</param>
        /// <returns>True if the passed node is the GameData folder, otherwise false.</returns>
        public static bool SetDestinationPaths(ModNode node, bool gameDataFound)
        {
            bool result = false;
            string path = string.Empty;
            ModNode tempNode = node;
            if (node.Text.ToLower() == Constants.GAMEDATA)
            {
                tempNode = node;
                path = KSPPathHelper.GetPath(KSPPaths.KSPRoot);
                result = true;
            }
            else if (node.Text.ToLower() == Constants.SHIPS)
            {
                tempNode = node;
                path = KSPPathHelper.GetPath(KSPPaths.KSPRoot);
                result = false;
            }
            else if (node.Text.ToLower() == Constants.VAB ||
                     node.Text.ToLower() == Constants.SPH)
            {
                tempNode = node;
                path = KSPPathHelper.GetPath(KSPPaths.Ships);
                result = false;
            }
            else if (gameDataFound || node.Parent == null)
            {
                tempNode = node;
                path = KSPPathHelper.GetPathByName(node.Name);
                path = (path.ToLower().EndsWith(node.Name.ToLower())) ? path.ToLower().Replace("\\" + node.Name.ToLower(), "") : path;
                result = false;
            }
            else
            {
                tempNode = (ModNode)node.Parent;
                path = KSPPathHelper.GetPath(KSPPaths.GameData);
                result = false;
            }

            SetDestinationPaths(tempNode, path);

            return result;
        }

        /// <summary>
        /// Builds and sets the destination path to the passed node and its childes.
        /// </summary>
        /// <param name="srcNode">Node to set the destination path.</param>
        /// <param name="destPath">The destination path.</param>
        /// <param name="copyContent"></param>
        public static void SetDestinationPaths(ModNode srcNode, string destPath, bool copyContent = false)
        {
            if (!copyContent)
            {
                //if (srcNode.Text.ToLower() == Constants.GAMEDATA)
                //    srcNode.Destination = destPath;
                //else
                srcNode.Destination = (!string.IsNullOrEmpty(destPath)) ? Path.Combine(destPath, srcNode.Text) : string.Empty;

                destPath = (!string.IsNullOrEmpty(srcNode.Destination)) ? srcNode.Destination : string.Empty;

                if (!string.IsNullOrEmpty(srcNode.Destination))
                    srcNode.Destination = KSPPathHelper.GetRelativePath(srcNode.Destination);
            }

            SetToolTips(srcNode);

            foreach (ModNode child in srcNode.Nodes)
                SetDestinationPaths(child, destPath);
        }

        /// <summary>
        /// Returns the destination path of the craft.
        /// </summary>
        /// <param name="craftNode">The craft to get the destination for.</param>
        /// <returns>The destination path of the craft.</returns>
        public static void SetCraftDestination(ModNode craftNode)
        {
            string zipPath = craftNode.ZipRoot.Key;

            using (IArchive archive = ArchiveFactory.Open(zipPath))
            {
                foreach (IArchiveEntry entry in archive.Entries)
                {
                    if (!entry.FilePath.EndsWith(craftNode.Text))
                        continue;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        entry.WriteTo(ms);
                        ms.Position = 0;
                        using (StreamReader sr = new StreamReader(ms))
                        {
                            string fullText = sr.ReadToEnd();
                            int index = fullText.IndexOf(TYPE);
                            if (index == -1)
                                continue;

                            string filename = Path.GetFileName(entry.FilePath);
                            if (string.IsNullOrEmpty(filename))
                                continue;

                            string shipType = fullText.Substring(index + 7, 3);
                            if (shipType.ToLower() == Constants.SPH)
                                craftNode.Destination = Path.Combine(KSPPathHelper.GetPath(KSPPaths.SPH), filename);
                            else
                                craftNode.Destination = Path.Combine(KSPPathHelper.GetPath(KSPPaths.VAB), filename);

                            if (!string.IsNullOrEmpty(craftNode.Destination))
                                craftNode.Destination = KSPPathHelper.GetRelativePath(craftNode.Destination);

                            SetToolTips(craftNode);

                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Set ToolTip

        /// <summary>
        /// Sets the ToolTip text of the node and all its childes.
        /// </summary>
        /// <param name="node">The node to set the ToolTip to.</param>
        public static void SetToolTips(ModNode node)
        {
            // TODO:!!!
            //if (node.Destination != string.Empty)
            //    node.ToolTipText = node.Destination.ToLower().Replace(MainForm.GetPath(KSP_Paths.KSP_Root).ToLower(), "KSP install folder");
            //else
            //    node.ToolTipText = "<No path selected>";

            //foreach (TreeNodeMod child in node.Nodes)
            //    SetToolTips(child);
        }

        #endregion

        #region Mod processing

        /// <summary>
        /// Processes all passed nodes. (Adds/Removes the MOD to/from the KSP install folders).
        /// </summary>
        /// <param name="mod">The mod to process.</param>
        /// <param name="silent">Determines if info messages should be added displayed.</param>
        /// <param name="overrideOn">Flag to set override of files on.</param>
        /// <param name="progressChanged">Callback function when the progress of the processing changed.</param>
        public static int ProcessMod(ModNode mod, bool silent = false, bool overrideOn = false, AsyncProgressChangedHandler progressChanged = null, int processedNodeCount = 0)
        {
            if (!silent)
            {
                Messenger.AddInfo(Constants.SEPARATOR);
                Messenger.AddInfo(string.Format(Messages.MSG_START_PROCESSING_0, mod.Name));
                Messenger.AddInfo(Constants.SEPARATOR);
            }

            int processedNode = processedNodeCount;
            try
            {
                processedNode = ProcessNodes(new ModNode[] { mod }, ref processedNode, silent, overrideOn, progressChanged);
            }
            catch (Exception ex)
            {
                Messenger.AddError(string.Format(Messages.MSG_ERROR_DURING_PROCESSING_MOD_0, mod.Name), ex);
            }

            DeleteNotProcessedDirectorys(silent);

            if (!silent)
                Messenger.AddInfo(Constants.SEPARATOR);

            return processedNode;
        }

        /// <summary>
        /// Processes all passed nodes. (Adds/Removes the MOD to/from the KSP install folders).
        /// </summary>
        /// <param name="modArray">The NodeArray to process.</param>
        /// <param name="silent">Determines if info messages should be added displayed.</param>
        /// <param name="overrideOn">Flag to set override of files on.</param>
        /// <param name="progressChanged">Callback function when the progress of the processing changed.</param>
        /// <param name="processedNodeCount">For internal use only!</param>
        private static int ProcessNodes(ModNode[] modArray, ref int processedNodeCount, bool silent = false, bool overrideOn = false, AsyncProgressChangedHandler progressChanged = null)
        {
            foreach (ModNode node in modArray)
            {
                if (node.HasDestination)
                {
                    if (!silent)
                        Messenger.AddInfo(string.Format(Messages.MSG_ROOT_IDENTIFIED, node.Text));

                    ProcessNode(node, ref processedNodeCount, silent, overrideOn, progressChanged);
                }
                else if (node.HasDestinationForChilds)
                {
                    if (progressChanged != null)
                        progressChanged(processedNodeCount += 1);

                    ModNode[] nodes = new ModNode[node.Nodes.Count];
                    for (int i = 0; i < node.Nodes.Count; ++i)
                        nodes[i] = (ModNode)node.Nodes[i];

                    ProcessNodes(nodes, ref processedNodeCount, silent, overrideOn, progressChanged);
                }
            }

            return processedNodeCount;
        }

        /// <summary>
        /// Processes the passed node. (Adds/Removes the MOD to/from the KSP install folders).
        /// </summary>
        /// <param name="node">The node to process.</param>
        /// <param name="silent">Determines if info messages should be added displayed.</param>
        /// <param name="overrideOn">Flag to set override of files on.</param>
        /// <param name="progressChanged">Callback function when the progress of the processing changed.</param>
        /// <param name="processedNodeCount">For internal use only!</param>
        private static void ProcessNode(ModNode node, ref int processedNodeCount, bool silent = false, bool overrideOn = false, AsyncProgressChangedHandler progressChanged = null)
        {
            if (node.Checked)
            {
                if (!File.Exists(node.ZipRoot.Key))
                {
                    if (!silent)
                        Messenger.AddInfo(string.Format(Messages.MSG_CANT_INSTALL_MODNODE_0_ZIP_MISSING, node.Destination));
                }
                else
                { 
                    if (!node.IsFile)
                        CreateDirectory(node, silent);
                    else
                        ExtractFile(node, node.Destination, silent, overrideOn);
                }
            }
            else
            {
                if (!node.IsFile)
                    RemoveDirectory(node, silent);
                else
                    RemoveFile(node, silent);
            }

            if (progressChanged != null)
                progressChanged(processedNodeCount += 1);

            foreach (ModNode child in node.Nodes)
                ProcessNode(child, ref processedNodeCount, silent, overrideOn, progressChanged);
        }

        /// <summary>
        /// Creates a directory for the ModNodes destination.
        /// </summary>
        /// <param name="node">The ModNode to get the destination of.</param>
        /// <param name="silent">Determines if info messages should be added displayed.</param>
        private static void CreateDirectory(ModNode node, bool silent)
        {
            string destination = node.Destination;
            if (!string.IsNullOrEmpty(destination))
                destination = KSPPathHelper.GetAbsolutePath(destination);

            if (!Directory.Exists(destination))
            {
                try
                {
                    Directory.CreateDirectory(destination);
                    if (!silent)
                        Messenger.AddInfo(string.Format(Messages.MSG_DIR_CREATED_0, destination));
                }
                catch
                {
                    Messenger.AddInfo(string.Format(Messages.MSG_DIR_CREATED_ERROR_0, destination));
                }
            }

            node.NodeType = (node.IsKSPFolder) ? NodeType.KSPFolderInstalled : NodeType.UnknownFolderInstalled;
        }

        /// <summary>
        /// Extracts the file from the archive with the passed key.
        /// </summary>
        /// <param name="node">The node to install the file from.</param>
        /// <param name="path">The path to install the file to.</param>
        /// <param name="silent">Determines if info messages should be added displayed.</param>
        private static void ExtractFile(ModNode node, string path, bool silent = false, bool overrideOn = false)
        {
            if (node == null) return;

            string destination = path;
            if (!string.IsNullOrEmpty(destination))
                destination = KSPPathHelper.GetAbsolutePath(destination);


            using (IArchive archive = ArchiveFactory.Open(node.ZipRoot.Key))
            {
                IArchiveEntry entry = archive.Entries.FirstOrDefault(e => e.FilePath == node.Key);
                if (entry == null)
                    return;

                if (!File.Exists(destination))
                {
                    try
                    {
                        // create new file.
                        entry.WriteToFile(destination);

                        if (!silent)
                            Messenger.AddInfo(string.Format(Messages.MSG_FILE_EXTRACTED_0, destination));
                    }
                    catch (Exception ex)
                    {
                        Messenger.AddError(string.Format(Messages.MSG_FILE_EXTRACTED_ERROR_0, destination), ex);
                    }
                }
                else if (overrideOn)
                {
                    try
                    {
                        // delete old file
                        File.Delete(destination);

                        // create new file.
                        entry.WriteToFile(destination);

                        if (!silent)
                            Messenger.AddInfo(string.Format(Messages.MSG_FILE_EXTRACTED_0, destination));
                    }
                    catch (Exception ex)
                    {
                        Messenger.AddError(string.Format(Messages.MSG_FILE_EXTRACTED_ERROR_0, destination), ex);
                    }
                }

                node.NodeType = NodeType.UnknownFileInstalled;
            }
        }

        /// <summary>
        /// Removes the directory the ModNodes destination points to.
        /// </summary>
        /// <param name="node">The ModNode to get the destination of.</param>
        /// <param name="silent">Determines if info messages should be added displayed.</param>
        private static void RemoveDirectory(ModNode node, bool silent)
        {
            if (!node.IsKSPFolder)
            {
                string destination = node.Destination;
                if (!string.IsNullOrEmpty(destination))
                    destination = KSPPathHelper.GetAbsolutePath(destination);

                if (Directory.Exists(destination))
                {
                    if (!Directory.GetDirectories(destination).Any() && !Directory.GetFiles(destination).Any())
                    {
                        try
                        {
                            Directory.Delete(destination, true);
                            if (!silent)
                                Messenger.AddInfo(string.Format(Messages.MSG_DIR_DELETED_0, destination));
                        }
                        catch
                        {
                            mNotDeletedDirs.Add(node);
                        }
                    }
                    else
                    {
                        // add dir for later try to delete
                        mNotDeletedDirs.Add(node);
                    }
                }
            }
            else
                mNotDeletedDirs.Add(node);

            node.NodeType = (node.IsKSPFolder) ? NodeType.KSPFolder : NodeType.UnknownFolder;
        }

        /// <summary>
        /// Removes the file the ModNodes destination points to.
        /// </summary>
        /// <param name="node">The ModNode to get the destination of.</param>
        /// <param name="silent">Determines if info messages should be added displayed.</param>
        private static void RemoveFile(ModNode node, bool silent)
        {
            string destination = node.Destination;
            if (!string.IsNullOrEmpty(destination))
                destination = KSPPathHelper.GetAbsolutePath(destination);

            bool installedByOtherMod = ModRegister.GetCollisionModFiles(node).Any(n => n.IsInstalled);
            if (File.Exists(destination) && !installedByOtherMod)
            {
                try
                {
                    File.Delete(destination);
                    if (!silent)
                        Messenger.AddInfo(string.Format(Messages.MSG_FILE_DELETED_0, destination));
                }
                catch (Exception ex)
                {
                    Messenger.AddError(string.Format(Messages.MSG_FILE_DELETED_ERROR_0, destination), ex);
                }
            }

            node.NodeType = NodeType.UnknownFile;
        }


        /// <summary>
        /// Try to delete all not processed directories.
        /// </summary>
        /// <param name="silent">Determines if info messages should be added displayed.</param>
        private static void DeleteNotProcessedDirectorys(bool silent = false)
        {
            var removedDirs = new List<ModNode>();
            mNotDeletedDirs.Sort((x, y) => y.Depth - x.Depth);
            foreach (ModNode node in mNotDeletedDirs)
                if (DeleteDirectory(node, silent))
                    removedDirs.Add(node);

            if (removedDirs.Count > 0)
            { 
                if (!silent)
                {
                    Messenger.AddInfo(Constants.SEPARATOR);
                    Messenger.AddInfo(Messages.MSG_DELETING_REMAINING_EMTPY_DIRS);
                    Messenger.AddInfo(Constants.SEPARATOR);
                }

                foreach (ModNode node in removedDirs)
                    mNotDeletedDirs.Remove(node);

                if (!silent)
                    Messenger.AddInfo(Constants.SEPARATOR);
            }
        }

        /// <summary>
        /// Try to delete all not processed directories.
        /// </summary>
        /// <param name="dir">The dir to delete.</param>
        /// <param name="silent">Determines if info messages should be added displayed.</param>
        private static bool DeleteDirectory(ModNode node, bool silent = false)
        {
            string destination = node.Destination;
            if (!string.IsNullOrEmpty(destination))
                destination = KSPPathHelper.GetAbsolutePath(destination);

            try
            {
                if (!Directory.Exists(destination))
                {
                    Messenger.AddInfo(string.Format(Messages.MSG_DIR_0_NOT_EXISTS, destination));
                    return false;
                }

                if (KSPPathHelper.IsKSPDir(destination))
                {
                    Messenger.AddInfo(string.Format(Messages.MSG_DIR_0_IS_KSPDIR, destination));
                    return false;
                }

                if (Directory.GetDirectories(destination).Any() ||
                    Directory.GetFiles(destination).Any())
                {
                    Messenger.AddInfo(string.Format(Messages.MSG_DIR_0_IS_NOT_EMPTY, destination));
                    return false;
                }

                Directory.Delete(destination, true);
                if (!silent)
                    Messenger.AddInfo(string.Format(Messages.MSG_DIR_DELETED_0, destination));

                return true;
            }
            catch (Exception ex)
            {
                Messenger.AddError(string.Format(Messages.MSG_DIR_DELETED_ERROR_0, destination), ex);
            }

            return false;
        }

        #endregion
    }
}
