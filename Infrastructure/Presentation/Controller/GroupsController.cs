//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using ServiceAbstraction.Contracts;
//using Shared.DTOs.GroupModule;
//using Shared.DTOs.Posts;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Presentation.Controller
//{
//    [Authorize]
//    public class GroupsController(IServiceManager serviceManager) : ApiController
//    {
//        [HttpGet("GetAllGroups")]
//        public async Task<ActionResult<List<GroupResultDTO>>> GetAllGroupsAsync()
//            => Ok(await serviceManager.GroupService.GetAllGroupsAsync());


//        [HttpGet("MyGroups")]
//        public async Task<ActionResult<List<GroupResultDTO>>> GetMyGroupsAsync()
//            => Ok(await serviceManager.GroupService.GetMyGroupsAsync(UserId));


//        [HttpGet("GetGroup/{Id:int}")]
//        public async Task<ActionResult<GroupDetailsResultDTO>> GetGroupByIdAsync(int Id)
//            => Ok(await serviceManager.GroupService.GetGroupByIdAsync(Id));


//        [HttpPost("CreateGroup")]

//        public async Task<ActionResult<GroupResultDTO>> CreateGroupAsync( CreateGroupDTO createGroupDTO)
//            => Ok(await serviceManager.GroupService.CreateGroupAsync(createGroupDTO, UserId));


//        [HttpPut("UpdateGroup/{Id:int}")]
//        public async Task<ActionResult<GroupResultDTO>> UpdateGroupAsync(int Id, [FromBody] UpdateGroupDTO updateGroupDTO)
//            => Ok(await serviceManager.GroupService.UpdateGroupAsync(Id, updateGroupDTO, UserId));


//        [HttpDelete("DeleteGroup/{Id:int}")]
//        public async Task<ActionResult> DeleteGroupAsync(int Id)
//            => Ok(await serviceManager.GroupService.DeleteGroupAsync(Id, UserId));


//        [HttpPut("UpdateGroupPicture/{Id:int}")]
//        //[Consumes("multipart/form-data")]
//        public async Task<ActionResult<GroupResultDTO>> UpdateGroupPictureAsync(int Id, [FromForm] UpdateGroupPictureDTO updateGroupPictureDTO)
//            => Ok(await serviceManager.GroupService.UpdateGroupPictureAsync(Id, updateGroupPictureDTO, UserId));


//        [HttpDelete("DeleteGroupPicture/{Id:int}")]
//        public async Task<ActionResult> DeleteGroupPictureAsync(int Id)
//            => Ok(await serviceManager.GroupService.DeleteGroupPictureAsync(Id, UserId));


//        [HttpGet("ExploreGroups")]
//        public async Task<ActionResult<PagedResult<GroupResultDTO>>> ExploreGroupsAsync(int page = 1, int pageSize = 20)
//            => Ok(await serviceManager.GroupService.ExploreGroupsAsync(page, pageSize, UserId));

//    }
//}
