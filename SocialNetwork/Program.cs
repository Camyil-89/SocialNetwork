using Microsoft.AspNetCore.Authentication.Cookies;
using SocialNetwork.Utilities.Logging;

namespace SocialNetwork
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Authorization/Login");
					options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Index");
				});

			builder.Services.AddAuthorization(options => options.AddPolicy("all", policy =>
				policy.RequireRole(new string[] { "user", "authorization" })
			));


			// Add services to the container.
			builder.Services.AddRazorPages();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapRazorPages();

			Service.DataBase.DataBaseProvider.Init("server=localhost;port=3306;username=webapi;password=!1N7XmccClyGXMOb;database=socialnetwork");

			app.Run();
		}
	}
}