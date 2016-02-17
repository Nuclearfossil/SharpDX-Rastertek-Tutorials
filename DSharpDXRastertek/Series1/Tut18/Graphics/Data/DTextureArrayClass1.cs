using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;

namespace DSharpDXRastertek.Tut18.Graphics.Data
{
    public class DTextureArray : ICollection<DTexture>                  // 92 lines
    {
        // Variables
        private Device _Device;

        // Properties
        public List<DTexture> TextureList { get; private set; }

        // Methods
        public bool Initialize(Device device, string[] fileNames)
        {
            try
            {
                // Load the texture file.
                TextureList = new List<DTexture>();
                _Device = device;

                foreach (var fileName in fileNames)
                    if (!AddFromFile(fileName))
                        return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void Shutdown()
        {
            // Release the texture resource.
            Clear();
        }
        public bool AddFromFile(string fileName)
        {
            DTexture texture = new DTexture();
            if (!texture.Initialize(_Device, fileName))
                return false;

            this.Add(texture);

            return true;
        }

        // ICollection Interface Methods
        public void Add(DTexture item)
        {
            TextureList?.Add(item);
        }
        public void Clear()
        {
            foreach (var texture in TextureList)
                texture?.ShutDown();

            TextureList.Clear();
        }
        public bool Contains(DTexture item)
        {
            throw new NotImplementedException();
        }
        public void CopyTo(DTexture[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        public int Count
        {
            get { return TextureList.Count; }
        }
        public bool IsReadOnly
        {
            get { return true; }
        }
        public bool Remove(DTexture item)
        {
            throw new NotImplementedException();
        }
        public IEnumerator<DTexture> GetEnumerator()
        {
            return TextureList.GetEnumerator();
        }
        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}