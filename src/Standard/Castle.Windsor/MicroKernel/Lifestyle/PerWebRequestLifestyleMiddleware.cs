// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


using System.Threading.Tasks;
using Castle.Windsor.MicroKernel.Lifestyle.Scoped;
using Microsoft.AspNetCore.Http;

namespace Castle.Windsor.MicroKernel.Lifestyle
{
    public class PerWebRequestLifestyleMiddleware
    {
        private const string key = "castle.per-web-request-lifestyle-cache";
        private readonly RequestDelegate _next;
        private HttpContext _context = null;

        public PerWebRequestLifestyleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            GetScope(context, true);

            await _next.Invoke(context);

            var scope = GetScope(context, false);
            if (scope != null)
                scope.Dispose();
        }

        private ILifetimeScope GetScope(HttpContext context, bool createIfNotPresent)
        {
            var candidates = (ILifetimeScope)context.Items[key];
            if (candidates == null && createIfNotPresent)
            {
                candidates = new DefaultLifetimeScope(new ScopeCache());
                context.Items[key] = candidates;
            }
            return candidates;
        }
    }
}