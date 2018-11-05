//using Identity.API.Data;
//using IdentityModel;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;

//namespace Identity.API.Managers
//{
//    public class ClaimsTransformer : IClaimsTransformation
//    {
//        private readonly ApplicationDbContext _context;

//        public ClaimsTransformer(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
//        {
//            var existingClaimsIdentity = (ClaimsIdentity)principal.Identity;
//            var currentUserName = existingClaimsIdentity.Name;

//            // Initialize a new list of claims for the new identity
//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.Name, currentUserName),
//                // Potentially add more from the existing claims here
//            };

//            // Find the user in the DB
//            // Add as many role claims as they have roles in the DB
//            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(existingClaimsIdentity.FindFirst(JwtClaimTypes.Subject).Value));
//            if (user != null)
//            {
//                var rolesNames = from ur in _context.UserRoles.Where(p => p.UserId == user.Id)
//                                 from r in _context.Roles
//                                 where ur.RoleId == r.Id
//                                 select r.Name;

//                claims.AddRange(rolesNames.Select(x => new Claim(ClaimTypes.Role, x)));
//                claims.Add(new Claim("email", user.Email));
//                claims.Add(new Claim("phone", user.Telefone));
//                claims.Add(new Claim("address", user.Endereco));
//                claims.Add(new Claim("address_details", user.Complemento));
//                claims.Add(new Claim("neighborhood", user.Bairro));
//                claims.Add(new Claim("city", user.Municipio));
//                claims.Add(new Claim("state", user.UF));
//                claims.Add(new Claim("zip_code", user.CEP));
//            }

//            // Build and return the new principal
//            var newClaimsIdentity = new ClaimsIdentity(claims, existingClaimsIdentity.AuthenticationType);
//            return new ClaimsPrincipal(newClaimsIdentity);
//        }
//    }
//}
