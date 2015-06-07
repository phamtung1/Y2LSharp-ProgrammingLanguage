using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Y2L_IDE
{
    class StringTable
    {
        private const string DEPENDENCIES_DIR = "_Dependencies";
        public const string FILE_PROJECT_EXTENSION = "y2lproj";
        public const string FILE_CODE_EXTENSION = "y2l";
        public const string FILE_EXE_EXTENSION = "yexe";
        public const string REGISTRY_PATH = @"Software\Y2L";
        public const string RECENT = "Recent";

        public const string DEFAULT_CODE_FILE = "Program." + FILE_CODE_EXTENSION;
        public const string TEMPLATES_DIR = DEPENDENCIES_DIR + "/" + "Templates";
        public const string CODESNIPPLET_DIR = DEPENDENCIES_DIR + "/" + "CodeSnipplets";
        public const string URL = "http://yinyangit.wordpress.com";
    }
    class Global
    {
        public static int ProjectsCreatedCount = 1;
        public static string ProjectName;
        public static string ProjectPath;

        public static List<string> Files = new List<string>();
        public static Y2List<string> RecentProjects;

        public static string GetFullProjectFilePath()
        {
            if (String.IsNullOrEmpty(ProjectPath) || String.IsNullOrEmpty(ProjectName))
                return null;
            return Path.Combine(ProjectPath, ProjectName + "." + StringTable.FILE_PROJECT_EXTENSION);
        }

    }
    class Y2List<T>:IEnumerable<T>
    {
        private List<T> _list;

        public Y2List()
        {
            _list = new List<T>();
        }
        public void AddFirst(T item)
        {
            if (_list.Contains(item))
                _list.Remove(item);
            _list.Insert(0, item);
        }
        public void AddLast(T item)
        {
            if (_list.Contains(item))
                _list.Remove(item);
            _list.Add(item);
        }
        public void Remove(T item)
        {
            if (_list.Contains(item))
            {
                _list.Remove(item);
            }
        }
        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }
        public int Count
        {
            get { return _list.Count; }
        }
        public T this[int index]
        {
            get { return _list[index]; }
        }
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        
    }
}
