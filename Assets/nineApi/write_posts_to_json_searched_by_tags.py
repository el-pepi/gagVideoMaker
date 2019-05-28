import os
import sys
import argparse

from nineapi.client import Client, APIException
from nineapi.client import Post, APIException
from nineapi.client import Group, APIException

def main(args):
    client = Client()
    client.username = os.environ['NINEGAG_USERNAME']
    client.password = os.environ['NINEGAG_PASSWORD']

    try:
        client.log_in(client.username, client.password)
    except APIException as e:
        print('Failed to log in:', e)
    else:
        query = args.tag + "?ref=featured-tag"
        posts = client.search_posts_by_tag(query,args.count,['animated','video'], args.sort)
        client.write_posts_to_json(posts)

if __name__ == '__main__':
    parser = argparse.ArgumentParser(
        description='write 9gag post info to json',
        add_help=True,
    )
    parser.add_argument('-t', '--tag', default="cat" )
    parser.add_argument('-c', '--count', default=10, type=int)
    parser.add_argument('-s', '--sort', default="asc")
    args = parser.parse_args()

    sys.exit(main(args))
