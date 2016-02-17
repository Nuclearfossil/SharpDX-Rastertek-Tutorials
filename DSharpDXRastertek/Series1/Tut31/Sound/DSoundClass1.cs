using SharpDX;
using SharpDX.DirectSound;
using System;

namespace DSharpDXRastertek.Tut31.Sound
{
    public class DSound                 // 115 lines
    {
        // Variables
        public DirectSound _DirectSound = null;
        PrimarySoundBuffer _PrimaryBuffer = null;
        private SoundListener3D _Listener = null;
        public SoundBuffer3D _3DSecondarySoundBuffer = null;
        string _AudioFileName = string.Empty;

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
                    Flags = BufferFlags.PrimaryBuffer | BufferFlags.ControlVolume | BufferFlags.Control3D,
                    AlgorithmFor3D = Guid.Empty
                };

                // Get control of the primary sound buffer on the default sound device.
                _PrimaryBuffer = new PrimarySoundBuffer(_DirectSound, primaryBufferDesc);

                 _Listener = new SoundListener3D(_PrimaryBuffer);
                _Listener.Deferred = false;
                _Listener.Position = new Vector3(0.0f, 0.0f, 0.0f);
            }
            catch (Exception)
            {
                return false;
            }

			return true;
        }
        private void ShutdownDirectSound()
        {
            // Release the listener interface.
            _Listener?.Dispose();
            _Listener = null;
            // Release the 3D Secondary Sound Buffer.
            _3DSecondarySoundBuffer?.Dispose();
            _3DSecondarySoundBuffer = null;
            // Release the primary sound buffer pointer.
            _PrimaryBuffer?.Dispose();
            _PrimaryBuffer = null;
            // Release the direct sound interface pointer.
            _DirectSound?.Dispose();
            _DirectSound = null;
        }
        public bool Play(int volume, Vector3 soundPosition)
        {
            return PlayAudioFile(volume, soundPosition);
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
        protected virtual bool PlayAudioFile(int volume, Vector3 soundPosition)
        {
            return true;
        }
    }
}