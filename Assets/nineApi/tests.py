from unittest import TestCase
from nineapi.client import Client, APIException
import os
import sys


class APITest(TestCase):
    def setUp(self):
        self.client = Client()
        self.username = "funnyvidsan"
        self.password = "pt181179"

    def test_log_in_good(self):
        response = self.client.log_in(self.username, self.password)
        self.assertEqual(response, True)

    def test_log_in_bad(self):
        self.assertRaises(APIException, lambda: self.client.log_in(self.username, self.password + 'wrong'))

    def test_get_posts(self):
        self.test_log_in_good()
        posts = self.client.get_posts()
        self.assertEqual(len(posts), 10)
