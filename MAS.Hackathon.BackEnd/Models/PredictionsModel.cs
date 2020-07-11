using System.Collections.Generic;

namespace MAS.Hackathon.BackEnd.Models
{
    public class PredictionsModel
    {
        public BoundingBoxModel BoundingBox { get; set; }
        public float Probability { get; set; }
        public int TagId { get; set; }
        public string TagName { get; set; }
    }
}
