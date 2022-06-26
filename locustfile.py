from locust import HttpUser, task, between, SequentialTaskSet
import urllib.request
import random
word_url = "https://www.mit.edu/~ecprice/wordlist.10000"
response = urllib.request.urlopen(word_url)
long_txt = response.read().decode()
words = long_txt.splitlines()


class ModifyUser(SequentialTaskSet):

    @task
    def insert_task(self):
        for i in range(0, len(words)):
            w = words[i]
            with self.client.post("/insert", json={"timestamp": 0, "id": w, "additionalProperties": "string"}, catch_response=True) as response:
                if response.status_code == 200:
                    response.success()
                elif response.status_code == 409:
                    response.success()
            self.client.post(f"/debugSearch?key={w}", name="/debugSearch")

    @task
    def stop(self):
        self.user.environment.reached_end = True
        self.user.environment.runner.quit()
            
class ApiUser(HttpUser):
    tasks = [ModifyUser]