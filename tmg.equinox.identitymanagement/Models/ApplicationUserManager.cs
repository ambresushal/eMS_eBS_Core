using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.identitymanagement.Models.Mapping;
using System.Linq;

namespace tmg.equinox.identitymanagement.Models
{

    /// <summary>
    /// Provides a kind of wrapper for managing user and related functionalities
    /// </summary>
    public class ApplicationUserManager : UserManager<ApplicationUser, int>
    {
        public ApplicationUserManager()
            : base(new UserStore<ApplicationUser, ApplicationRole, int, ApplicationUserLogin, ApplicationUserRole
                , ApplicationRoleClaim>(new SecurityDbContext()))
        {
            PasswordValidator = new PasswordValidator(3, 10);
            PasswordValidator = new Microsoft.AspNet.Identity.MinimumLengthValidator(3);
            this.PasswordHasher = new SQLPasswordHasher();
        }

        //needed for OWIN authentication
        public ApplicationUserManager(IUserStore<ApplicationUser, int> store)
            : base(store)
        {
            PasswordValidator = new Microsoft.AspNet.Identity.MinimumLengthValidator(3);
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            // Configure the userstore to use the DbContext to work with database
            //var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<IdentityDbContext>()));


            var manager = new ApplicationUserManager(new ApplicationUserStore(new SecurityDbContext()));
            manager.PasswordValidator = new Microsoft.AspNet.Identity.MinimumLengthValidator(3);
            // The password validator enforces complexity on supplied password
            manager.PasswordValidator = new PasswordValidator(3, 10);

            // Use the custom password hasher to validate existing user credentials
            manager.PasswordHasher = new SQLPasswordHasher() { DbContext = new SecurityDbContext() };

            return manager;
        }
      
    }
    public class PasswordValidator : IIdentityValidator<string>
    {
        public int MinimumLength { get; private set; }
        public int MaximumLength { get; private set; }

        public PasswordValidator(int minimumLength, int maximumLength)
        {
            this.MinimumLength = minimumLength;
            this.MaximumLength = maximumLength;
        }
        public Task<IdentityResult> ValidateAsync(string item)
        {
            if (!string.IsNullOrWhiteSpace(item) && item.Trim().Length >= MinimumLength &&
                item.Trim().Length <= MaximumLength)
                return Task.FromResult(IdentityResult.Success);
            else return Task.FromResult(IdentityResult.Failed("Password did not meet requrements."));

        }
    }
    public class ApplicationRoleManager : RoleManager<ApplicationRole, int>
    {
        public ApplicationRoleManager()
            : base(new RoleStore<ApplicationRole, int, ApplicationUserRole>(new SecurityDbContext()))
        {

        }

        public ApplicationRoleManager(IRoleStore<ApplicationRole, int> store)
            : base (store)
        {

        }
        private static RoleManager<IdentityRole> newRoleManager
        {
            get { return new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new SecurityDbContext())); }
        }

        public static bool RoleExists(string roleName)
        {
            using (var roleManager = newRoleManager)
            {
                return roleManager.RoleExists<IdentityRole, string>(roleName);
            }
        }

        /// <summary>
        /// Needed for Owin
        /// </summary>
        /// <param name="options"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ApplicationRoleManager Create(
            IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<ApplicationRole,int,ApplicationUserRole>(context.Get<SecurityDbContext>()));
        }
    }
    /// <summary>
    /// Provides the ability for encryption and hasing etc
    /// </summary>
    public class SQLPasswordHasher : PasswordHasher
    {
        public SecurityDbContext DbContext { get; set; }

        public override string HashPassword(string password)
        {
            return base.HashPassword(password);
        }

        public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            string[] passwordProperties = hashedPassword.Split('|');
            if (passwordProperties.Length != 3)
            {
                return base.VerifyHashedPassword(hashedPassword, providedPassword);
            }
            else
            {
                string passwordHash = passwordProperties[0];
                int passwordformat = 1;
                string salt = passwordProperties[2];
                if (String.Equals(EncryptPassword(providedPassword, passwordformat, salt), passwordHash, StringComparison.CurrentCultureIgnoreCase))
                {
                    return PasswordVerificationResult.SuccessRehashNeeded;
                }
                else
                {
                    return PasswordVerificationResult.Failed;
                }
            }
        }

        //Provides the ability to encrypt password and this is copied from the existing SQL providers 
        //and is provided only for back-compatatibilty
        private string EncryptPassword(string pass, int passwordFormat, string salt)
        {
            if (passwordFormat == 0) // MembershipPasswordFormat.Clear
                return pass;

            byte[] bIn = Encoding.Unicode.GetBytes(pass);
            byte[] bSalt = Convert.FromBase64String(salt);
            byte[] bRet = null;

            if (passwordFormat == 1)
            { // MembershipPasswordFormat.Hashed
                HashAlgorithm hm = HashAlgorithm.Create("HMACSHA256");
                KeyedHashAlgorithm kha = hm as KeyedHashAlgorithm;
                if (kha != null)
                {
                    if (kha.Key.Length == bSalt.Length)
                    {
                        kha.Key = bSalt;
                    }
                    else if (kha.Key.Length < bSalt.Length)
                    {
                        byte[] bKey = new byte[kha.Key.Length];
                        Buffer.BlockCopy(bSalt, 0, bKey, 0, bKey.Length);
                        kha.Key = bKey;
                    }
                    else
                    {
                        byte[] bKey = new byte[kha.Key.Length];
                        for (int iter = 0; iter < bKey.Length; )
                        {
                            int len = Math.Min(bSalt.Length, bKey.Length - iter);
                            Buffer.BlockCopy(bSalt, 0, bKey, iter, len);
                            iter += len;
                        }
                        kha.Key = bKey;
                    }
                    bRet = kha.ComputeHash(bIn);
                }
                else
                {
                    byte[] bAll = new byte[bSalt.Length + bIn.Length];
                    Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
                    Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);
                    bRet = hm.ComputeHash(bAll);
                }
            }
            else
            {
                byte[] bAll = new byte[bSalt.Length + bIn.Length];
                Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
                Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);
                // bRet = EncryptPassword(bAll, LegacyPasswordCompatibilityMode);
            }

            return Convert.ToBase64String(bRet);
        }

    }
}
