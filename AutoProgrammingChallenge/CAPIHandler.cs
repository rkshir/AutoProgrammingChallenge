using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace APIClient
{
    /// <summary>
    /// API handler class. 
    /// Has functions to call the corresponding APIs
    /// </summary>
    public class APIHandler
    {
        /// <summary>
        /// Global variable for base URL
        /// </summary>
        private Uri m_ApiURL = new Uri("http://vautointerview.azurewebsites.net/");
        /// <summary>
        /// 
        /// </summary>
        public List<int> UniqueDealerIDs { get; set; }
        /// <summary>
        /// get dataset
        /// </summary>
        /// <returns>DatasetDetails</returns>
        public DatasetDetails GetDatasetID()
        {
            DatasetDetails _dsDatasetDetails = new DatasetDetails();
            Console.WriteLine("Getting Dataset...");
            string _sApiRequest = "/api/datasetId";
            Console.WriteLine("Calling API: " + m_ApiURL + _sApiRequest);
            try
            {
                HttpClientHandler _HttpClientHandler = new HttpClientHandler();
                HttpClient _HttpClient = new HttpClient(_HttpClientHandler);
                _HttpClient.DefaultRequestHeaders.Accept.Clear();
                _HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _HttpClient.BaseAddress = m_ApiURL;

                HttpResponseMessage _HttpResponseMessage = _HttpClient.GetAsync(_sApiRequest).Result;
                if (_HttpResponseMessage.IsSuccessStatusCode)
                {
                    _dsDatasetDetails = _HttpResponseMessage.Content.ReadAsAsync<DatasetDetails>().Result;                    
                }
                Console.WriteLine("Received Dataset.");
                Console.WriteLine("Dataset is: " + _dsDatasetDetails.datasetId);
            }
            catch (Exception)
            {
                throw;
            }
            return _dsDatasetDetails;
        }

        /// <summary>
        /// get all vehicles for the dataset
        /// </summary>
        /// <param name="DatasetID"></param>
        /// <returns>vehicle list</returns>
        public List<Vehicle> GetVehicles(string DatasetID)
        {
            List<Vehicle> _lstVehicles = new List<Vehicle>();
            try
            {
                Console.WriteLine("Getting vehicles list...");
                string _sApiRequest = string.Format("/api/{0}/vehicles", DatasetID);
                Console.WriteLine("Calling API: " + m_ApiURL + _sApiRequest);
                VehicleIDs _cVehicleIDs = new VehicleIDs();

                HttpClientHandler _HttpClientHandler = new HttpClientHandler();
                HttpClient _HttpClient = new HttpClient(_HttpClientHandler);
                _HttpClient.DefaultRequestHeaders.Accept.Clear();
                _HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _HttpClient.BaseAddress = m_ApiURL;

                HttpResponseMessage _HttpResponseMessage = _HttpClient.GetAsync(_sApiRequest).Result;
                if (_HttpResponseMessage.IsSuccessStatusCode)
                {
                    _cVehicleIDs = _HttpResponseMessage.Content.ReadAsAsync<VehicleIDs>().Result;
                }
                Console.WriteLine("Received vehicles list.");
            
                int _iVehicleCount = _cVehicleIDs.vehicleIds.Count();
                Console.WriteLine("Starting thread to get vehicle details...");

                // declare the threads to get vehicle details
                Thread[] _workerThread = new Thread[_iVehicleCount];
                object[] _oReturnValue = new object[_iVehicleCount];
                WaitHandle[] _workerThreadWaitHandles = new WaitHandle[_iVehicleCount];

                // foreach (int vID in vehicleIDs.vehicleIds)
                for (int _iCount = 0; _iCount < _iVehicleCount; _iCount++)
                {
                    var _vHandleCount = _iCount;

                    var _vEventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                    _workerThread[_iCount] = new Thread(() => { _oReturnValue[_iCount] = GetVehicleInfo(DatasetID, _cVehicleIDs.vehicleIds[_iCount]); _vEventWaitHandle.Set(); });
                    _workerThreadWaitHandles[_vHandleCount] = _vEventWaitHandle;
                    _workerThread[_iCount].Start();
                    Thread.Sleep(25);
                }
                Console.WriteLine("Waiting for thread finish getting the vehicle details...");
                WaitHandle.WaitAll(_workerThreadWaitHandles);

                UniqueDealerIDs = new List<int>();
                for (int _iRetValCount = 0; _iRetValCount < _iVehicleCount; _iRetValCount++)
                {
                    Vehicle _TempVehicle = (Vehicle)_oReturnValue[_iRetValCount];
                    _lstVehicles.Add((Vehicle)_oReturnValue[_iRetValCount]);
                    // get unique dealer IDs
                    if (!UniqueDealerIDs.Contains(_TempVehicle.dealerId)) UniqueDealerIDs.Add(_TempVehicle.dealerId);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _lstVehicles;
        }


        public List<Dealer> GetDealerInfo(string DatasetID,  List<int> DealerIDs)
        {
            List<Dealer> _lstDealer = new List<Dealer>();
            try
            {
                int _iDealerCount = DealerIDs.Count();
                // declare the threads to get vehicle details
                Thread[] _workerThread = new Thread[_iDealerCount];
                object[] _oReturnValue = new object[_iDealerCount];
                WaitHandle[] _WaitHandle = new WaitHandle[_iDealerCount];

                // foreach (int vID in vehicleIDs.vehicleIds)
                for (int _iCount = 0; _iCount < _iDealerCount; _iCount++)
                {
                    var _vHandleCount = _iCount;

                    var _vEventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                    _workerThread[_iCount] = new Thread(() => { _oReturnValue[_iCount] = GetDealerInfo(DatasetID, DealerIDs[_iCount]); _vEventWaitHandle.Set(); });
                    _WaitHandle[_vHandleCount] = _vEventWaitHandle;
                    _workerThread[_iCount].Start();
                    Thread.Sleep(25);
                }
                Console.WriteLine("Waiting for thread finish getting the vehicle details...");
                WaitHandle.WaitAll(_WaitHandle);
                for (int _iRetValCount = 0; _iRetValCount < _iDealerCount; _iRetValCount++)
                {
                    _lstDealer.Add((Dealer)_oReturnValue[_iRetValCount]);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _lstDealer;
        }

        /// <summary>
        /// get vehicle information
        /// </summary>
        /// <param name="datasetID"></param>
        /// <param name="VehicleID"></param>
        /// <returns>vhicle information</returns>
        private Vehicle GetVehicleInfo(string datasetID, int VehicleID)
        {
            Vehicle _cVehicle = new Vehicle();
            Console.WriteLine("Getting vehicles details for VehicleId: " + VehicleID.ToString());
            string _sApiRequest = string.Format("/api/{0}/vehicles/{1}", datasetID, VehicleID);
            Console.WriteLine("Calling API: " + m_ApiURL + _sApiRequest);

            try
            {
                HttpClientHandler _HttpClientHandler = new HttpClientHandler();
                HttpClient _HttpClient = new HttpClient(_HttpClientHandler);
                _HttpClient.DefaultRequestHeaders.Accept.Clear();
                _HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _HttpClient.BaseAddress = m_ApiURL;

                HttpResponseMessage _HttpResponseMessage = _HttpClient.GetAsync(_sApiRequest).Result;
                if (_HttpResponseMessage.IsSuccessStatusCode)
                {
                    _cVehicle = _HttpResponseMessage.Content.ReadAsAsync<Vehicle>().Result;
                }
                Console.WriteLine("Finished getting vehicles details for VehicleId: " + VehicleID.ToString());
            }
            catch (Exception)
            {
                throw;
            }
            return _cVehicle;
        }
        /// <summary>
        /// get dealer info
        /// </summary>
        /// <param name="DatasetID"></param>
        /// <param name="DealerID"></param>
        /// <returns>delaer info</returns>
        public Dealer GetDealerInfo(string DatasetID, int DealerID)
        {
            Dealer _cDealer = new Dealer();
            Console.WriteLine("Getting dealer details for DealerID: " + DealerID.ToString());
            string _sApiRequest = string.Format("/api/{0}/dealers/{1}", DatasetID, DealerID);
            Console.WriteLine("Calling API: " + m_ApiURL + _sApiRequest);

            try
            {
                HttpClientHandler _HttpClientHandler = new HttpClientHandler();
                HttpClient _HttpClient = new HttpClient(_HttpClientHandler);
                _HttpClient.DefaultRequestHeaders.Accept.Clear();
                _HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _HttpClient.BaseAddress = m_ApiURL;

                HttpResponseMessage _HttpResponseMessage = _HttpClient.GetAsync(_sApiRequest).Result;
                if (_HttpResponseMessage.IsSuccessStatusCode)
                {
                    _cDealer = _HttpResponseMessage.Content.ReadAsAsync<Dealer>().Result;
                }
                Console.WriteLine("Finished getting dealer details for DealerID: " + DealerID.ToString());
            }
            catch (Exception)
            {
                throw;
            }
            return _cDealer;
        }
        /// <summary>
        /// Posts the answer to the server by calling the API
        /// </summary>
        /// <param name="DatasetID"></param>
        /// <param name="Answer"></param>
        /// <returns>response from the API</returns>
        public AnswerResponse PostAnswer(string DatasetID, Answer Answer)
        {
            AnswerResponse _cAnswerResponse = new AnswerResponse();
            Console.WriteLine("Posting answer for the dataset: " + DatasetID);
            string _sApiRequest = string.Format("/api/{0}/answer", DatasetID);
            Console.WriteLine("Calling API: " + m_ApiURL + _sApiRequest);

            try
            {
                HttpClientHandler _HttpClientHandler = new HttpClientHandler();
                HttpClient _HttpClient = new HttpClient(_HttpClientHandler);
                _HttpClient.DefaultRequestHeaders.Accept.Clear();
                _HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _HttpClient.BaseAddress = m_ApiURL;

                HttpResponseMessage _HttpResponseMessage = _HttpClient.PostAsJsonAsync(_sApiRequest, Answer).Result;
                if (_HttpResponseMessage.IsSuccessStatusCode)
                {
                    _cAnswerResponse = _HttpResponseMessage.Content.ReadAsAsync<AnswerResponse>().Result;
                }
                Console.WriteLine("Finished posting answer for dataset: " + DatasetID);
            }
            catch (Exception)
            {
                throw;
            }
            return _cAnswerResponse;
        }
    }
}
