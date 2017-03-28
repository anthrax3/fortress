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
using Microsoft.AspNetCore.Http;

namespace Castle.MicroKernel.Lifestyle
{
    // This is not yet complete
    public class PerWebRequestLifestyleMiddleware
    {
        private readonly RequestDelegate _next;

        public PerWebRequestLifestyleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next.Invoke(context);
            // Thinking right now is release policies can be implemented directly on the container
        }
    }
}