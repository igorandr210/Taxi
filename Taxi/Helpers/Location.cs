using System;
using Geolocation;
using NetTopologySuite.Geometries;
using Taxi.Models.Trips;

namespace Taxi.Helpers
{
    public static class Location
    {
        const double EarthRadiusM = 6371.010;

        public static double CalculateKilometersDistance(double lat1, double lon1, double lat2, double lon2)
        {
            return GeoCalculator.GetDistance(lat1, lon1, lat2, lon2, 5, DistanceUnit.Kilometers);
        }
        public static double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }
        private static double ConvertToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
        public static Point pointFromLatLng(double lat, double lon)
        {
            lat = ConvertToRadians(lat);
            lon = ConvertToRadians(lon);
            return new Point(EarthRadiusM * Math.Cos(lat) * Math.Cos(lon),
                EarthRadiusM * Math.Cos(lat) * Math.Sin(lon),
                EarthRadiusM * Math.Sin(lat));
        }

        public static PlaceDto PointToPlaceDto(Point p)
        {
            var s = p.SRID;
            if (s != 4326)
                return null;
            return new PlaceDto()
            {
                Longitude = p.X,
                Latitude = p.Y
            };
        }
        //public static PlaceDto CartesianToSpherical(Point p)
        //{
        //    //if (cartCoords.X == 0)
        //    //    cartCoords.X = double.Epsilon;
        //    //var outRadius = Math.Sqrt((cartCoords.X * cartCoords.X)
        //    //                + (cartCoords.Y * cartCoords.Y)
        //    //                + (cartCoords.Z * cartCoords.Z));
        //    //var outPolar = Math.Atan(cartCoords.Z / cartCoords.X);
        //    //if (cartCoords.X < 0)
        //    //    outPolar += Math.PI;
        //    //  outElevation = Mathf.Asin(cartCoords.y / outRadius);
        //    if (p.Coordinate.X == 0)
        //        p.Coordinate.X = double.Epsilon;
        //        var radius = Math.Sqrt(p.Coordinate.X * p.Coordinate.X +
        //        p.Coordinate.Y * p.Coordinate.Y +
        //        p.Coordinate.Z * p.Coordinate.Z);
        //    var latitude = Math.Asin(p.Coordinate.Z / radius);
        //    var longitude = Math.Atan2(p.Coordinate.Y, p.Coordinate.X);
        //    latitude = ConvertToDegree(latitude);
        //    longitude = ConvertToDegree(longitude);
        //    return new PlaceDto()
        //    {
        //        Latitude = latitude,
        //        Longitude = longitude
        //    };
        //}
    }
}
