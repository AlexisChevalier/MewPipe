using System;
using MewPipe.DAL.Models;
using MewPipe.DAL.Models.Oauth;

namespace MewPipe.DAL.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private readonly MewPipeDbContext _context = new MewPipeDbContext();
        private GenericRepository<User> _userRepository;
        private GenericRepository<OauthAccessToken> _oauthAccessTokenRepository;
        private GenericRepository<OauthRefreshToken> _oauthRefreshTokenRepository;
        private GenericRepository<OauthAuthorizationCode> _oauthAuthorizationCodeRepository;
        private GenericRepository<OauthUserTrust> _oauthUserTrustRepository;
        private GenericRepository<OauthClient> _oauthClientRepository;

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
