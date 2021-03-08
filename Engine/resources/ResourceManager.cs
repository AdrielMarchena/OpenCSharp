using Engine.audio;
using Engine.render;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Engine.resources
{
    /// <summary>
    /// Template class to create a Resource Manager
    /// </summary>
    /// <typeparam name="K"> Key </typeparam>
    /// <typeparam name="T"> Type </typeparam>
    public class ResourceManager<K, T>
    {
        private const char Map = 'M';
        private const char Sound = 'S';
        private const char Image = 'I';

        private Dictionary<K, T> m_resource_list;

        public ResourceManager()
        {
            m_resource_list = new Dictionary<K, T>();
        }

        public void AddResource(K key, T resource)
        {
            m_resource_list[key] = resource;
        }

        public T PopResource(K key)
        {
            if (m_resource_list.ContainsKey(key))
            {
                var tmp = m_resource_list[key];
                m_resource_list.Remove(key);
                return tmp;
            }
            throw new Exception("No resource whith: < " + key + " > key was found!");
        }
        public T GetResource(K key)
        {
            if (m_resource_list.ContainsKey(key))
                return m_resource_list[key];
            throw new Exception("No resource whith < " + key + " > key was found!");
        }

        public T this[K key]
        {
            get { return m_resource_list[key]; }
            set { m_resource_list[key] = value;  }
        }

        public void RemoveResource(K key)
        {
            if (m_resource_list.ContainsKey(key))
                m_resource_list.Remove(key);
        }

    }
}
