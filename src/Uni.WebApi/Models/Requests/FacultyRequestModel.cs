﻿using JetBrains.Annotations;

namespace Uni.WebApi.Models.Requests
{
    [PublicAPI]
    public class FacultyRequestModel
    {
        public int UniversityId { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string Description { get; set; }
    }
}
