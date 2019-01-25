// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace Identity.API
{
    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("Carrinho.API", "Carrinho da CasaDoCodigo"),
                new ApiResource("CasaDoCodigo.API", "API CasaDoCodigo"),
                new ApiResource("OrdemDeCompra.API", "Ordem de Compra da CasaDoCodigo")
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients(string callbackUrl)
        {
            // client credentials client
            return new List<Client>
            {
                // OpenID Connect hybrid flow and client credentials client (MVC)
                new Client
                {
                    ClientId = "MVC",
                    ClientName = "MVC",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    RequireConsent = false,
                    ClientSecrets = 
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = { callbackUrl + "signin-oidc" },
                    PostLogoutRedirectUris = { callbackUrl + "signout-callback-oidc" },
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "Carrinho.API",
                        "CasaDoCodigo.API",
                        "OrdemDeCompra.API"
                    },
                    AllowOfflineAccess = true
                }
            };
        }
    }
}