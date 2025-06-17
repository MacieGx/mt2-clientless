using System.Text;
using MetinClientless.Packets;

namespace MetinClientless;

public class SqlQueryBuilder
{
    public static string BuildInsertQuery(PacketGCShopContent packetGCShopContent)
    {
        StringBuilder query = new StringBuilder();
        query.Append("INSERT INTO items (shop_id, item_id, pos, count, yang, wons, total_price, region, shop_transaction_id, item_transaction_id, attributes, sockets, has_built_in_attribute) VALUES ");

        for (int i = 0; i < packetGCShopContent.Items.Count; i++)
        {
            var item = packetGCShopContent.Items[i];
            
            query.AppendFormat("({0}, {1}, {2}, {3}, {4}, {5}, {6}, '{7}', {8}, {9}, '{10}'::jsonb, '{11}'::jsonb, {12})",
                packetGCShopContent.Id, item.Vnum, item.Pos, item.Count, item.Price, 0, item.Price, Configuration.Account.Region, packetGCShopContent.ShopTransactionId, item.ItemTransactionId, item.BuildAttributesJson(), item.BuildSocketsJson(), item.HasBuiltInAttribute);

            if (i < packetGCShopContent.Items.Count - 1)
            {
                query.Append(", ");
            }
        }

        query.Append("ON CONFLICT (shop_id, pos, region) DO UPDATE SET " +
                     "item_id = EXCLUDED.item_id, count = EXCLUDED.count, yang = EXCLUDED.yang, " +
                     "wons = EXCLUDED.wons, total_price = EXCLUDED.total_price, updated_at = CURRENT_TIMESTAMP, " +
                     "shop_transaction_id = EXCLUDED.shop_transaction_id, item_transaction_id = EXCLUDED.item_transaction_id, " +
                     "attributes = EXCLUDED.attributes, sockets = EXCLUDED.sockets, " + 
                     "has_built_in_attribute = EXCLUDED.has_built_in_attribute;");

        return query.ToString();
    }
}