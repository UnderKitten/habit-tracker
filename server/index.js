const express = require("express");
const app = express();

app.use(express.json());

app.get("/api/health", (request, response) => {
  response.json({
    status: "OK",
  });
});

const PORT = 3001;
// app.listen(PORT);
// console.log(`Server runnng on port ${PORT}`);
app.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
});
