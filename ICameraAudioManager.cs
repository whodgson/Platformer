using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.script
{
    public interface ICameraAudioManager
    {
        CameraAudioManager UpdateCameraAudioController();
    }

    public class CameraAudioManager
    {
        public bool is_submerged;

        public CameraAudioManager()
        {
            is_submerged = false;
        }
    }
}
