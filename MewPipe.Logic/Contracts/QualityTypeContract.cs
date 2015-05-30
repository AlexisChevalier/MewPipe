using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MewPipe.Logic.Models;

namespace MewPipe.Logic.Contracts
{
    public class QualityTypeContract
    {
        public QualityTypeContract()
        {
        }

        public QualityTypeContract(QualityType qualityType)
        {
            Name = qualityType.Name;
        }

        public string Name { get; set; }
    }
}
