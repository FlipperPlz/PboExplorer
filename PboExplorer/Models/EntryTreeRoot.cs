﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using PboExplorer.Utils.Interfaces;
using PboExplorer.Utils.Managers;
using PboExplorer.Utils.Repositories;

namespace PboExplorer.Models; 

public class EntryTreeRoot : ITreeRoot {
    private readonly ObservableCollection<ITreeItem> _entryList = new();
    
    public IEnumerable<TreeDataEntry> Files => ((ITreeEnumerable)this).Files;
    public IEnumerable<TreeDirectoryEntry> Directories => ((ITreeEnumerable)this).Directories;
    
    public EntryTreeManager TreeManager { get; set; }
    public EntryDataRepository DataRepository { get; set; }

    public EntryTreeRoot(EntryTreeManager treeManager) {
        TreeManager = treeManager;
        DataRepository = treeManager.DataRepository;
    }
    
    public ICollection<ITreeItem>? TreeChildren {
        get => _entryList;
        set {
            _entryList.Clear();
            if (value == null) return;
            foreach (var treeItem in value)
                _entryList.Add(treeItem);
        }
    }

    public ITreeItem AddChild(ITreeItem child) {
        _entryList.Add(child);
        
        return child;
    }

    public void RemoveChild(ITreeItem child) {
        if(!_entryList.Contains(child)) return;
        
        _entryList.Remove(child);
    }

    public IEnumerable<TreeDataEntry> RecursivelyGrabAllFiles() => 
        Directories.SelectMany(d => d.RecursivelyGrabAllFiles()).Concat(Files);
    
    public T GetOrCreateChild<T>(string title) where T : ITreeItem => ((ITreeEnumerable)this).GetOrCreateChild<T>(title);

}