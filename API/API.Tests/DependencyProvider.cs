using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Xunit.Abstractions;

namespace API.Tests
{
    class TestWrapper : IWrapper
    {
        public void AddPaginationHeaderViaWrapper(HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
        }

        public string GetUsernameViaWrapper(ClaimsPrincipal user)
        {
            return "USERNAME";
        }
    }
    public class DependencyProvider
    {
        protected static IMapper _mapper = null!;
        protected static IUnitOfWork _fakeUnitOfWork = null!;
        protected static ITokenService _fakeTokenService = null!;
        protected static IPhotoService _fakePhotoService = null!;
        protected static IWrapper _wrapper = null!;
        public DependencyProvider()
        {
            var mapingConfig = new MapperConfiguration(mc => 
            {
                mc.AddProfile(new AutoMapperProfiles());
            });
            _mapper = mapingConfig.CreateMapper();
            _fakeUnitOfWork = A.Fake<IUnitOfWork>();
            _fakeTokenService = A.Fake<ITokenService>();
            _fakePhotoService = A.Fake<IPhotoService>();
            _wrapper = new TestWrapper();
        }
    }
}