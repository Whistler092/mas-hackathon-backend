using System.Collections.Generic;

namespace MAS.Hackathon.BackEnd.Models
{
    public class MainModel
    {
        public string Created { get; set; }
        public string Id { get; set; }
        public string Project { get; set; }
        public List<PredictionsModel> Predictions { get; set; }
    }
}
