using System;

namespace Shelter2250.Gameplay
{
    [Serializable]
    public sealed class ResourceMeter
    {
        public ResourceKind kind;
        public float current;
        public float max;
        public float productionPerSecond;

        public ResourceMeter(ResourceKind kind, float current, float max, float productionPerSecond = 0f)
        {
            this.kind = kind;
            this.current = current;
            this.max = max;
            this.productionPerSecond = productionPerSecond;
        }
    }
}

