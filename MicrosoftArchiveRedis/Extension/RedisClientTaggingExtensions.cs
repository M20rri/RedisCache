using System.Collections.Generic;
using System.Threading.Tasks;
using MicrosoftArchiveRedis.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MicrosoftArchiveRedis.Extension
{
    public static class RedisClientTaggingExtensions
    {
        public static async Task SaveCacheKeyTags(this IDatabase client, string key, int? organizationId, int? promotionId, int? organizationPromotionId)
        {

            if (organizationId.HasValue)
            {
                await client.SetAddAsync($"o-{organizationId.Value}", key);
            }
            if (promotionId.HasValue)
            {
                await client.SetAddAsync($"p-{promotionId.Value}", key);
            }
            if (organizationPromotionId.HasValue)
            {
                await client.SetAddAsync($"op-{organizationPromotionId.Value}", key);
            }

        }

        public static async Task FindAllCacheKeyByTag(this IDatabase client, string ownerContextType, int? ownerEntityId)
        {
            string tagKey = (ownerContextType == "Organization") ? $"o-{ownerEntityId}" : (
                            (ownerContextType == "Promotion") ? $"p-{ownerEntityId}" : $"op-{ownerEntityId}"
                            );


            RedisValue[] members = await client.SetMembersAsync(tagKey);
            foreach (var member in members)
            {
                if (member.HasValue)
                    await client.KeyDeleteAsync(member.ToString());
            }

            await client.KeyDeleteAsync(tagKey);
        }

    }
}