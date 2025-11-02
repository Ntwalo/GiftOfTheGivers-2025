import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

export let errorRate = new Rate('errors');

export let options = {
    vus: 50,          // start with 50 virtual users
    duration: '30s',  // run 30 seconds
    thresholds: {
        'errors': ['rate<0.01'],      // less than 1% errors
        'http_req_duration': ['p(95)<1000'] // 95% requests below 1s
    }
};

export default function () {
    let res = http.get('https://your-app-url/'); // replace with your deployed/test url
    let ok = check(res, { 'status 200': (r) => r.status === 200 });
    errorRate.add(!ok);
    sleep(1);
}
