using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MewPipe.Logic.Models;
using MewPipe.Logic.Repositories;

namespace MewPipe.Logic.Services
{
    public interface IVideoQualityTypeService
    {
        QualityType GetQualityType(string qualityType);
        QualityType GetDefaultQualityType();
        QualityType GetUploadingQualityType();
        QualityType[] GetEncodingQualityTypes();
    }

    public class VideoQualityTypeService : IVideoQualityTypeService
    {
        private readonly UnitOfWork _unitOfWork;

        public VideoQualityTypeService(UnitOfWork unitOfWork = null)
        {
            _unitOfWork = unitOfWork ?? new UnitOfWork();
        }

        public QualityType GetQualityType(string qualityType)
        {
            var mime = _unitOfWork.QualityTypeRepository.GetOne(q => q.Name == qualityType);

            return mime;
        }

        public QualityType GetDefaultQualityType()
        {
            var mime = _unitOfWork.QualityTypeRepository.GetOne(q => q.IsDefault);

            return mime;
        }

        public QualityType GetUploadingQualityType()
        {
            var mime = _unitOfWork.QualityTypeRepository.GetOne(q => q.IsUploaded);

            return mime;
        }

        public QualityType[] GetEncodingQualityTypes()
        {
            var mimes = _unitOfWork.QualityTypeRepository.Get(q => !q.IsUploaded).ToArray();

            return mimes;
        }
    }
}
