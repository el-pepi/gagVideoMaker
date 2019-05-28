import os
from nineapi.client import Client, APIException
from nineapi.client import Post, APIException
from nineapi.client import Group, APIException

client = Client()
client.username = os.environ['NINEGAG_USERNAME']
client.password = os.environ['NINEGAG_PASSWORD']

try:
    client.log_in(client.username, client.password)
except APIException as e:
    print('Failed to log in:', e)
else:
    groups = client.get_all_groups()
    client.write_groups_to_json(groups)