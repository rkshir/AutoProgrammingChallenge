using System;
using System.Collections.Generic;
using System.Linq;

namespace APIClient
{
    /// <summary>
    /// Main class
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main program class
        /// </summary>
        /// <param name="args"></param>
        static int Main(string[] args)
        {
            try
            {
                APIHandler _APIHandler = new APIHandler();
                DatasetDetails _dsDatasetDetails = _APIHandler.GetDatasetID();
                if (!String.IsNullOrEmpty(_dsDatasetDetails.datasetId))
                {
                    List<Vehicle> _lstVehicles = _APIHandler.GetVehicles(_dsDatasetDetails.datasetId);
                    List<Dealer> _lstDealers = _APIHandler.GetDealerInfo(_dsDatasetDetails.datasetId, _APIHandler.UniqueDealerIDs);
                    var _vDealerGroup = (from _vVehicle in _lstVehicles.GroupBy(m => m.dealerId)
                                        join _vDealer in _lstDealers
                                         on _vVehicle.Key equals _vDealer.dealerId
                                         select new Dealer()
                                         {
                                             dealerId = _vDealer.dealerId,
                                             name = _vDealer.name,
                                             vehicles = _vVehicle.Select(dv => new DealerVehicle()
                                                                        {
                                                                            make = dv.make,
                                                                            model = dv.model,
                                                                            vehicleId = dv.vehicleId,
                                                                            year = dv.year
                                                                        }).ToList()
                                         });

                    Answer _Answer = new Answer();
                    _Answer.dealers = _vDealerGroup.ToList();
                    AnswerResponse _AnswerResponse = _APIHandler.PostAnswer(_dsDatasetDetails.datasetId, _Answer);
                    if (_AnswerResponse.success)
                    {
                        Console.WriteLine("Response Message: " + _AnswerResponse.message);
                        Console.WriteLine("Response Time (milliseconds): " + _AnswerResponse.totalMilliseconds.ToString());
                        Console.ReadLine();
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+ Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.Source);
                return 1;
            }
        }
    }
}
