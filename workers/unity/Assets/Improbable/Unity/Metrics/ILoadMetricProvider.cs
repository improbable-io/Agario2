namespace Improbable.Metrics
{
    public interface ILoadMetricProvider
    {
        /// <summary>
        /// Should report a number between 0 and 1 that represents how loaded the worker is,
        /// where 0 is not loaded at all, and 1 means that it cannot take any more load.
        /// This number is used for load balancing and scaling by SpatialOS.
        /// Note that numbers above 1 can still be reported and can be useful manual inspection,
        /// however, they won't add any additional meaning to SpatialOS.
        /// </summary>
        float GetLoad();
    }
}
