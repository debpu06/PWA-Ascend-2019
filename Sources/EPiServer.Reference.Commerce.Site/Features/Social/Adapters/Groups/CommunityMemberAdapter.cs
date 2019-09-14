using EPiServer.Reference.Commerce.Site.Features.Social.ExtensionData.Membership;
using EPiServer.Reference.Commerce.Site.Features.Social.Models.Groups;
using EPiServer.Social.Groups.Core;

namespace EPiServer.Reference.Commerce.Site.Features.Social.Adapters.Groups
{
    /// <summary>
    /// Adapts data to and from the community member.
    /// </summary>
    public class CommunityMemberAdapter
    {
        /// <summary>
        /// Adapts CommunityMember into an AddMemberRequest
        /// </summary>
        /// <param name="member">The CommunityMember to be adapted</param>
        /// <returns>AddMemberRequest</returns>
        public AddMemberRequest Adapt(CommunityMember member)
        {
            return new AddMemberRequest(member.GroupId, member.User, member.Email, member.Company);
        }

        /// <summary>
        /// Adapts an AddMemberRequest into a CommunityMember
        /// </summary>
        /// <param name="memberRequest">The AddMemberRequest to be adapted</param>
        /// <returns>CommunityMember</returns>
        public CommunityMember Adapt(AddMemberRequest memberRequest)
        {
            return new CommunityMember(memberRequest.User, memberRequest.Group, memberRequest.Email, memberRequest.Company);
        }

        /// <summary>
        /// Adapts a Member into a CommunityMember
        /// </summary>
        /// <param name="member">The member to be adapted </param>
        /// <param name="extension"></param>
        /// <returns>CommunityMember</returns>
        public CommunityMember Adapt(Member member, MemberExtensionData extension)
        {
            return new CommunityMember(member.User.Id, member.Group.Id, extension.Email, extension.Company);
        }
    }
}