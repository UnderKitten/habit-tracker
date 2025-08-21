import express from "express";
import mongoose from "mongoose";

const app = express();

app.use(express.json());

if (process.argv.length < 3) {
  console.log("give password as argument");
  process.exit(1);
}

const password = process.argv[2];
const url = `mongodb+srv://mchurzin:${password}@cluster0.az7dbro.mongodb.net/habitTracker?retryWrites=true&w=majority&appName=Cluster0`;

mongoose.set("strictQuery", false);
mongoose
  .connect(url)
  .then(() => {
    console.log("Connected to MongoDB");
  })
  .catch((error) => {
    console.error("Error connecting to MongoDB: ", error.message);
  });

app.get("/api/health", (request, response) => {
  response.json({
    status: "OK",
  });
});

const PORT = 3001;
app.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
});
