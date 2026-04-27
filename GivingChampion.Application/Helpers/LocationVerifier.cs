namespace GivingChampion.Application.Helpers
{
    public static class LocationVerifier
    {
        private const double EarthRadiusMeters = 6371000;

        public static double CalculateDistanceInMeters(
            double latitude1,
            double longitude1,
            double latitude2,
            double longitude2)
        {
            var lat1Rad = ToRadians(latitude1);
            var lat2Rad = ToRadians(latitude2);

            var deltaLat = ToRadians(latitude2 - latitude1);
            var deltaLon = ToRadians(longitude2 - longitude1);

            var a =
                Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusMeters * c;
        }

        public static bool IsWithinDistance(
            double userLatitude,
            double userLongitude,
            double targetLatitude,
            double targetLongitude,
            double thresholdMeters = 50)
        {
            var distance = CalculateDistanceInMeters(
                userLatitude,
                userLongitude,
                targetLatitude,
                targetLongitude);

            return distance <= thresholdMeters;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}