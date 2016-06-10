using System;

namespace Assets.Improbable.Core.Physical
{
    public class ThresholdBasedUpdateNotifier<T>
    {
        public event Action<float, T> ShouldUpdate;

        private readonly float updatePeriodThreshold;
        private readonly Func<T, T, bool> isValuePastThreshold;
        private float lastUpdateTime;
        private T lastValue;

        public ThresholdBasedUpdateNotifier(float updatePeriodThreshold, Func<T, T, bool> isValuePastThreshold, float currentTime, T initialValue)
        {
            this.updatePeriodThreshold = updatePeriodThreshold;
            this.isValuePastThreshold = isValuePastThreshold;
            Reset(currentTime, initialValue);
        }

        public void Reset(float currentTime, T initialValue)
        {
            lastUpdateTime = currentTime;
            lastValue = initialValue;
        }

        public void RegisterNewValue(float currentTime, T newValue)
        {
            if (IsLastUpdateTimePastThreshold(currentTime) && isValuePastThreshold(lastValue, newValue))
            {
                OnShouldUpdate(currentTime, newValue);
            }
        }

        private void OnShouldUpdate(float currentTime, T newValue)
        {
            if (ShouldUpdate != null)
            {
                ShouldUpdate(currentTime - lastUpdateTime, newValue);
                lastUpdateTime = currentTime;
                lastValue = newValue;
            }
        }

        private bool IsLastUpdateTimePastThreshold(float currentTime)
        {
            return (currentTime - lastUpdateTime) >= updatePeriodThreshold;
        }
    }
}
