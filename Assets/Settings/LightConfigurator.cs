using UnityEngine;

namespace Settings {
    public class LightConfigurator {
        private static int CompareLightRenderMode(LightRenderMode mode1, LightRenderMode mode2) {
            if (mode1 == mode2) {
                return 0;
            }

            if (mode1 == LightRenderMode.ForcePixel) {
                return -1;
            }

            if (mode2 == LightRenderMode.ForcePixel) {
                return 1;
            }

            if (mode1 == LightRenderMode.Auto) {
                return -1;
            }

            if (mode2 == LightRenderMode.Auto) {
                return 1;
            }
            return 0;
        }
    }
}