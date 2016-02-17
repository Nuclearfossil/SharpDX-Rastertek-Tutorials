using SharpDX.DirectSound;
using SharpDX.Multimedia;
using System;

namespace DSharpDXRastertek.Tut14.Sound
{
    public class DSound                 // 106 lines
    {
        // Variables
        public DirectSound _DirectSound;
        PrimarySoundBuffer _PrimaryBuffer;
        string _AudioFileName;

        // Constructor
        public DSound(string fileName)
        {
            _AudioFileName = fileName;
        }

        // Public Methods
        public bool Initialize(IntPtr windowHandle)
		{
            // Initialize direct sound and the primary sound buffer.
            if(!InitializeDirectSound(windowHandle))
				return false;

			return true;
        }
        public void Shutdown()
		{
			// Release the secondary buffer.
			ShutdownAudioFile();

			// Shutdown the Direct Sound API
			ShutdownDirectSound();
		}
        // Private Methods.
        private bool InitializeDirectSound(IntPtr windowHandler)
        {
            try
            {
                // Initialize the direct sound interface pointer for the default sound device.
                _DirectSound = new DirectSound();

                try
                {
                    _DirectSound.SetCooperativeLevel(windowHandler, CooperativeLevel.Priority);
                }
                catch
                {
                    return false;
                }

                // Setup the primary buffer description.
                SoundBufferDescription primaryBufferDesc = new SoundBufferDescription()
                {
                    Flags = BufferFlags.PrimaryBuffer | BufferFlags.ControlVolume,
                    AlgorithmFor3D = Guid.Empty
                };

                // Get control of the primary sound buffer on the default sound device.
                _PrimaryBuffer = new PrimarySoundBuffer(_DirectSound, primaryBufferDesc);

                // Setup the format of the primary sound buffer.
                // In this case it is a .
                _PrimaryBuffer.Format = new WaveFormat(44100, 16, 2);
            }
            catch (Exception)
            {
                return false;
            }

			return true;
        }
        private void ShutdownDirectSound()
        {
            // Release the primary sound buffer pointer.
            _PrimaryBuffer?.Dispose();
            _PrimaryBuffer = null;
            // Release the direct sound interface pointer.
            _DirectSound?.Dispose();
            _DirectSound = null;
        }
        public bool Play(int volume)
        {
            return PlayAudioFile(volume);
        }
        public bool LoadAudio(DirectSound directSound)
        {
            return LoadAudioFile(_AudioFileName, directSound);
        }

        // Virtual Methods
        protected virtual bool LoadAudioFile(string audioFile, DirectSound directSound)
        {
            return true;
        }
        protected virtual void ShutdownAudioFile()
        {
        }
        protected virtual bool PlayAudioFile(int volume)
        {
            return true;
        }
    }
}