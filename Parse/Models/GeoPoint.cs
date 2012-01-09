namespace Parse
{
   public class GeoPoint
   {
      public double Latitude { get; set; }
      public double Longitude { get; set; }

      public GeoPoint() { }
      public GeoPoint(double latitude, double longitude)
      {
         Latitude = latitude;
         Longitude = longitude;
      }

      /// <summary>
      /// Strictly used by the query engine
      /// </summary>
      public bool NearSphere(double latitude, double longitude)
      {
         return NearSphere(latitude, longitude, null);
      }

      /// <summary>
      /// Strictly used by the query engine
      /// </summary>
      public bool NearSphere(double latitude, double longitude, double? maxDistance)
      {
         return false;
      }
   }
}