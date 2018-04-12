using Newtonsoft.Json;
using System.Collections.Generic;

namespace APIClient
{
    /// <summary>
    /// class for dataset details
    /// </summary>
    public class DatasetDetails
    {
        /// <summary>
        /// dataset ID
        /// </summary>
        public string datasetId;
    }
    /// <summary>
    /// Answer to the server
    /// </summary>
    public class Answer
    {
        /// <summary>
        /// List of dealers
        /// </summary>
        public List<Dealer> dealers { get; set;}
    }
    /// <summary>
    /// Dealer details
    /// </summary>
    public class Dealer
    {
        /// <summary>
        /// dealer ID
        /// </summary>
        public int dealerId;
        /// <summary>
        /// dealer name
        /// </summary>
        public string name;
        /// <summary>
        /// list of vehicles what the dealer has
        /// </summary>
        public List<DealerVehicle> vehicles { get; set;}
    }
    /// <summary>
    /// vehicle details
    /// </summary>
    public class Vehicle
    {
        /// <summary>
        /// vehicle id
        /// </summary>
        public int vehicleId { get; set; }
        /// <summary>
        /// vehicle year
        /// </summary>
        public int year { get; set; }
        /// <summary>
        /// vehicle make
        /// </summary>
        public string make { get; set; }
        /// <summary>
        /// vehicle model
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// dealer id
        /// </summary>
        public int dealerId { get; set; }
        /// <summary>
        /// dealer info
        /// </summary>
    }
    /// <summary>
    /// vehicle ids
    /// </summary>
    public class VehicleIDs
    {
        /// <summary>
        /// list of vehicle ids
        /// </summary>
        public int[] vehicleIds;
    }
    /// <summary>
    /// answer
    /// </summary>
    public class AnswerResponse
    {
        /// <summary>
        /// success or failure that is returned from server
        /// </summary>
        public bool success;
        /// <summary>
        /// return message from server
        /// </summary>
        public string message;
        /// <summary>
        /// time taken to execute the whole process
        /// </summary>
        public int totalMilliseconds;
    }
    /// <summary>
    /// dealer vehicle details
    /// </summary>
    public class DealerVehicle
    {
        /// <summary>
        /// vehicle ID
        /// </summary>
        public int vehicleId;
        /// <summary>
        /// vehicle year
        /// </summary>
        public int year;
        /// <summary>
        /// vehicle make
        /// </summary>
        public string make;
        /// <summary>
        /// vehicle model
        /// </summary>
        public string model;
    }
}
