using System;
using System.Data.Entity.Core.Common.CommandTrees;
using MewPipe.Logic.Models;
using MewPipe.Logic.Models.Oauth;

namespace MewPipe.Logic.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private readonly MewPipeDbContext _context = new MewPipeDbContext();
        private GenericRepository<User> _userRepository;
        private GenericRepository<Video> _videoRepository;
        private GenericRepository<VideoFile> _videoFileRepository;
        private GenericRepository<QualityType> _qualityTypeRepository;
        private GenericRepository<MimeType> _mimeTypeRepository;
        private GenericRepository<Impression> _impressionRepository;
        private GenericRepository<Tag> _tagRepository;
        private GenericRepository<Category> _categoryRepository;

        private GenericRepository<VideoUploadToken> _videoUploadTokenRepository;
        private GenericRepository<OauthAccessToken> _oauthAccessTokenRepository;
        private GenericRepository<OauthRefreshToken> _oauthRefreshTokenRepository;
        private GenericRepository<OauthAuthorizationCode> _oauthAuthorizationCodeRepository;
        private GenericRepository<OauthUserTrust> _oauthUserTrustRepository;
        private GenericRepository<OauthClient> _oauthClientRepository;

        public GenericRepository<Impression> ImpressionRepository
        {
            get
            {

                if (_impressionRepository == null)
                {
                    _impressionRepository = new GenericRepository<Impression>(_context);
                }
                return _impressionRepository;
            }
        }

        public GenericRepository<Tag> TagRepository
        {
            get
            {

                if (_tagRepository == null)
                {
                    _tagRepository = new GenericRepository<Tag>(_context);
                }
                return _tagRepository;
            }
        }

        public GenericRepository<Category> CategoryRepository
        {
            get
            {

                if (_categoryRepository == null)
                {
                    _categoryRepository = new GenericRepository<Category>(_context);
                }
                return _categoryRepository;
            }
        }

        public GenericRepository<User> UserRepository
        {
            get
            {

                if (_userRepository == null)
                {
                    _userRepository = new GenericRepository<User>(_context);
                }
                return _userRepository;
            }
        }
        
        public GenericRepository<Video> VideoRepository
        {
            get
            {

                if (_videoRepository == null)
                {
                    _videoRepository = new GenericRepository<Video>(_context);
                }
                return _videoRepository;
            }
        }

        public GenericRepository<QualityType> QualityTypeRepository
        {
            get
            {

                if (_qualityTypeRepository == null)
                {
                    _qualityTypeRepository = new GenericRepository<QualityType>(_context);
                }
                return _qualityTypeRepository;
            }
        }

        public GenericRepository<VideoFile> VideoFileRepository
        {
            get
            {

                if (_videoFileRepository == null)
                {
                    _videoFileRepository = new GenericRepository<VideoFile>(_context);
                }
                return _videoFileRepository;
            }
        }

        public GenericRepository<MimeType> MimeTypeRepository
        {
            get
            {

                if (_mimeTypeRepository == null)
                {
                    _mimeTypeRepository = new GenericRepository<MimeType>(_context);
                }
                return _mimeTypeRepository;
            }
        }

        public GenericRepository<VideoUploadToken> VideoUploadTokenRepository
        {
            get
            {

                if (_videoUploadTokenRepository == null)
                {
                    _videoUploadTokenRepository = new GenericRepository<VideoUploadToken>(_context);
                }
                return _videoUploadTokenRepository;
            }
        }

        public GenericRepository<OauthAccessToken> OauthAccessTokenRepository
        {
            get
            {

                if (_oauthAccessTokenRepository == null)
                {
                    _oauthAccessTokenRepository = new GenericRepository<OauthAccessToken>(_context);
                }
                return _oauthAccessTokenRepository;
            }
        }

        public GenericRepository<OauthRefreshToken> OauthRefreshTokenRepository
        {
            get
            {

                if (_oauthRefreshTokenRepository == null)
                {
                    _oauthRefreshTokenRepository = new GenericRepository<OauthRefreshToken>(_context);
                }
                return _oauthRefreshTokenRepository;
            }
        }

        public GenericRepository<OauthAuthorizationCode> OauthAuthorizationCodeRepository
        {
            get
            {

                if (_oauthAuthorizationCodeRepository == null)
                {
                    _oauthAuthorizationCodeRepository = new GenericRepository<OauthAuthorizationCode>(_context);
                }
                return _oauthAuthorizationCodeRepository;
            }
        }

        public GenericRepository<OauthUserTrust> OauthUserTrustRepository
        {
            get
            {

                if (_oauthUserTrustRepository == null)
                {
                    _oauthUserTrustRepository = new GenericRepository<OauthUserTrust>(_context);
                }
                return _oauthUserTrustRepository;
            }
        }

        public GenericRepository<OauthClient> OauthClientRepository
        {
            get
            {

                if (_oauthClientRepository == null)
                {
                    _oauthClientRepository = new GenericRepository<OauthClient>(_context);
                }
                return _oauthClientRepository;
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        #region Dispose Members

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
