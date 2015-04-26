using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MewPipe.Logic.Models;
using MewPipe.Logic.Repositories;

namespace MewPipe.Logic.Services
{
    public interface IVideoMimeTypeService
    {
        MimeType GetAllowedMimeTypeForDecoding(string mimeType);
        MimeType GetEncodingMimeType(string mimeType);
        MimeType GetDefaultEncodingMimeType();
        MimeType[] GetEncodingMimeTypes();
    }

    public class VideoMimeTypeService : IVideoMimeTypeService
    {
        private readonly UnitOfWork _unitOfWork;

        public VideoMimeTypeService(UnitOfWork unitOfWork = null)
        {
            _unitOfWork = unitOfWork ?? new UnitOfWork();
        }

        public MimeType GetAllowedMimeTypeForDecoding(string mimeType)
        {
            var mime = _unitOfWork.MimeTypeRepository.GetOne(m => m.HttpMimeType == mimeType && m.AllowedForDecoding);

            return mime;
        }

        public MimeType GetEncodingMimeType(string mimeType)
        {
            var mime = _unitOfWork.MimeTypeRepository.GetOne(m => m.HttpMimeType == mimeType && m.RequiredForEncoding);

            return mime;
        }

        public MimeType GetDefaultEncodingMimeType()
        {
            var mime = _unitOfWork.MimeTypeRepository.GetOne(m => m.IsDefault && m.RequiredForEncoding);

            return mime;
        }

        public MimeType[] GetEncodingMimeTypes()
        {
            var mimes = _unitOfWork.MimeTypeRepository.Get(m => m.RequiredForEncoding);

            return mimes.ToArray();
        }
    }
}
