﻿using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
        public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "None")
        {
            string sql = "EXEC TutorialAppSchema.spPosts_Get";
            string stringParameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            if (postId != 0)
            {
                stringParameters += ", @PostId = @PostIdParam";
                sqlParameters.Add("@PostIdParam",postId,System.Data.DbType.Int32);
            }
            if (userId != 0)
            {
                stringParameters += ", @UserId=@UserIdParam";
                sqlParameters.Add("@UserIdParam", userId, System.Data.DbType.Int32);
            }
            if (searchParam.ToLower() != "none")
            {
                stringParameters += ", @SearchValue=@SearchParam";
                sqlParameters.Add("@SearchParam", searchParam, System.Data.DbType.String);
            }
            if(stringParameters.Length > 0)
            {
                sql += stringParameters.Substring(1);
            }

            return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sql = "EXEC TutorialAppSchema.spPosts_Get @UserId = @UserIdParam";
            DynamicParameters sqlParamaters = new DynamicParameters();

            sqlParamaters.Add ("@UserIdParam", this.User.FindFirst("userId")?.Value, System.Data.DbType.Int32);

            return _dapper.LoadDataWithParameters<Post>(sql, sqlParamaters);
        }

        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post postToUpsert)
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
                                    @UserId = @UserIdParam,
                                    @PostTitle = @PostTitleParam,
                                    @PostContent = @PostContentParam";
            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@UserIdParam", this.User.FindFirst("userId")?.Value, System.Data.DbType.Int32);
            sqlParameters.Add("@PostTitleParam", postToUpsert.PostTitle, System.Data.DbType.String);
            sqlParameters.Add("@PostContentParam", postToUpsert.PostContent, System.Data.DbType.String);

            if (postToUpsert.PostId > 0) 
            {
                sql += ", @PostId = @PostIdParam";
                sqlParameters.Add("@PostIdParam", postToUpsert.PostId, System.Data.DbType.Int32);
            }                            

            if (_dapper.ExecuteSqlWithParameters(sql,sqlParameters))
            {
                return Ok();
            }

            throw new Exception("Failed to upsert post");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = "EXEC TutorialAppSchema.spPost_Delete @PostId = @PostIdParam, @UserId = @UserIdParam";
            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@PostIdParam", postId, System.Data.DbType.Int32);
            sqlParameters.Add("@UserIdParam", this.User.FindFirst("userId")?.Value, System.Data.DbType.Int32);

            if (_dapper.ExecuteSqlWithParameters(sql,sqlParameters))
            {
                return Ok();
            }

            throw new Exception("Failed to delete post");
        }
    }
}
