using System;
using System.Collections.Generic;
using UnityEditor.Experimental;

namespace Scenes
{
    [Serializable]
    public class FaceData
    {
        public List<Face> faces;
        public int count;
        public List<Location> locations;
        
    }
    
    [Serializable]
    public class Location
    {
        public int bottom;
        public int left;
        public int right;
        public int top;
    }
    
    [Serializable]
    public class Face
    {
        public float dist;
        public int id;
    }
}