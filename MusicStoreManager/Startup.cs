using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicStoreManager.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MusicStoreManager.Services;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json.Serialization;
using MusicStoreManager.ApiClient;
using MusicStoreManager.Entities;
using System.Reflection;
using System.IO;
using MusicStoreManager.Services;

namespace MusicStoreManager
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<MusicStoreApiClient>();

            services.AddDbContext<mvcMusicStoreContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<IAlbumRepository, AlbumRepository>();
            services.AddTransient<IFeedbackRepository, FeedbackRepository>();
            services.AddTransient<IGenreRepository, GenreRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IArtistRepository, ArtistRepository>();
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<ITypeHelperService, TypeHelperService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<ShoppingCart>(sp => ShoppingCart.GetCart(sp));
            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory =>
            {
                var actionContext =
                implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddMvc()
                .AddMvcOptions(opt =>
                {
                    opt.ReturnHttpNotAcceptable = true;
                    opt.InputFormatters.Add(new XmlDataContractSerializerInputFormatter(opt));
                    opt.InputFormatters.OfType<JsonInputFormatter>().FirstOrDefault()
                        .SupportedMediaTypes.Add("application/vnd.musicstore.hateos+json");
                    opt.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                    opt.OutputFormatters.OfType<JsonOutputFormatter>().FirstOrDefault()
                        .SupportedMediaTypes.Add("application/vnd.musicstore.hateos+json");
                })

                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });


            //services.AddMemoryCache();
            services.AddSession();
            //services.AddHttpCacheHeaders(
            //    (expirationModelOptions) =>
            //    {
            //        expirationModelOptions.MaxAge = 600;
            //    },
            //    (validationModelOptions) =>
            //    {
            //        validationModelOptions.MustRevalidate = true;
            //    });

            //services.AddResponseCaching();


        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionHandlerFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global exception logger");
                            logger.LogError(500, exceptionHandlerFeature.Error, exceptionHandlerFeature.Error.Message); 
                        }
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");

                    });
                });
                //app.UseExceptionHandler("/AppException");
            }

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Entities.Album, Models.AlbumDto>()
                .ForMember(o => o.Artist, ex => ex.MapFrom(o => o.Artist.Name))
                .ForMember(o => o.Genre, ex => ex.MapFrom(o => o.Genre.Name))
                .ReverseMap();

                cfg.CreateMap<Models.AlbumDto, ViewModels.AlbumViewModel>()
                .ReverseMap();

                cfg.CreateMap<Entities.Artist, Models.ArtistDto>()
                .ReverseMap();

                cfg.CreateMap<Models.ArtistForCreation, Entities.Artist>();

                cfg.CreateMap<Models.AlbumForCreation, Entities.Album>();

                cfg.CreateMap<Models.AlbumForUpdate, Entities.Album>()
                .ReverseMap();

                
            });

            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();

            //app.UseResponseCaching();
            //app.UseHttpCacheHeaders();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "albumDetails",
                    template: "Album/Details/{id}/{artistId}",
                    defaults: new { Controller = "Album", action = "Details" });

                routes.MapRoute(
                    name: "Default",
                    template: "{controller=Home}/{action=Index}");
            });
        }
    }
}
