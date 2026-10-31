import http from "k6/http";

export const options = {
    vus: 100, // 100 Users
    duration: '2m' // 2 minutes
}

export default function() {
    http.get("http://airbnb.gateway:8080/api/audit/users/all")
}