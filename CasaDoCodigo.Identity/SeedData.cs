// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using CasaDoCodigo.Identity.Data;
using CasaDoCodigo.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CasaDoCodigo.Identity
{
    public class SeedData
    {
        public static void EnsureSeedData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.Migrate();

                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var alice = userMgr.FindByNameAsync("alice").Result;
                if (alice == null)
                {
                    alice = new ApplicationUser
                    {
                        UserName = "alice",
                        Nome = "Alice Smith",
                        Telefone = "1234-5678",
                        Endereco = "Rua Vergueiro, 456",
                        Complemento = "8 andar sala 801",
                        Bairro = "Vila Mariana",
                        Municipio = "São Paulo",
                        UF = "SP",
                        CEP = "69118"
                    };
                    var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userMgr.AddClaimsAsync(alice, new Claim[]{
                        new Claim("name", "Alice Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Alice"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        new Claim("address", "Rua Vergueiro, 456"),
                        new Claim("address_details", "8 andar sala 801"),
                        new Claim("phone", "1234-5678"),
                        new Claim("neighborhood", "Vila Mariana"),
                        new Claim("city", "São Paulo"),
                        new Claim("state", "SP"),
                        new Claim("zip_code", "69118")
                    }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Console.WriteLine("alice created");
                }
                else
                {
                    Console.WriteLine("alice already exists");
                }

                var bob = userMgr.FindByNameAsync("bob").Result;
                if (bob == null)
                {
                    bob = new ApplicationUser
                    {
                        UserName = "bob",
                        Nome = "Bob Smith",
                        Telefone = "1234-5678",
                        Endereco = "Rua Vergueiro, 456",
                        Complemento = "8 andar sala 801",
                        Bairro = "Vila Mariana",
                        Municipio = "São Paulo",
                        UF = "SP",
                        CEP = "69118"
                    };
                    var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userMgr.AddClaimsAsync(bob, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Bob Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Bob"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                        new Claim("location", "somewhere"),
                        new Claim("address", "Rua Vergueiro, 456"),
                        new Claim("address_details", "8 andar sala 801"),
                        new Claim("phone", "1234-5678"),
                        new Claim("neighborhood", "Vila Mariana"),
                        new Claim("city", "São Paulo"),
                        new Claim("state", "SP"),
                        new Claim("zip_code", "69118")
                    }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Console.WriteLine("bob created");
                }
                else
                {
                    Console.WriteLine("bob already exists");
                }
            }
        }
    }
}
