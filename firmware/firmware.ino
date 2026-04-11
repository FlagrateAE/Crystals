// Flagrate Crystals Arduino UNO R3 controller

// Serial format:
// R.G.B. - jump to color
// R.G.B~ - fade to color smoothly

const int RED_PIN = 11;
const int GREEN_PIN = 10;
const int BLUE_PIN = 9;

// Speed of the transition (lower is faster)
const int FADE_SPEED = 5;

// Store current colors to know where the next fade starts
int R = 0;
int G = 0;
int B = 0;

void setup() {
  Serial.begin(9600);
  pinMode(RED_PIN, OUTPUT);
  pinMode(GREEN_PIN, OUTPUT);
  pinMode(BLUE_PIN, OUTPUT);

  onStart();
}

void onStart() {
  setColor(0, 0, 0);
  fadeToColor(255, 255, 255, 30);

  Serial.println("Flagrate Crystals: online");
}

void loop() {
  if (Serial.available() > 0) {
    String input = Serial.readStringUntil('\n');
    input.trim();

    if (input.length() > 0) {
      processCommand(input);
    }
  }
}

void processCommand(String cmd) {
  char lastChar = cmd.charAt(cmd.length() - 1);

  // split the string
  int firstDot = cmd.indexOf('.');
  int secondDot = cmd.indexOf('.', firstDot + 1);

  if (firstDot != -1 && secondDot != -1) {
    int r = cmd.substring(0, firstDot).toInt();
    int g = cmd.substring(firstDot + 1, secondDot).toInt();

    // for blue, we read from the second dot to the character before the suffix (~ or .)
    int b = cmd.substring(secondDot + 1, cmd.length() - 1).toInt();

    if (lastChar == '~') {
      // Smooth transition
      fadeToColor(r, g, b, FADE_SPEED);
      // Serial.print("Fading to: ");
    } else {
      // Momentary transition
      R = r;
      G = g;
      B = b;
      setColor(R, G, B);
      // Serial.print("Jumping to: ");
    }

    // Serial.print(r);
    // Serial.print(".");
    // Serial.print(g);
    // Serial.print(".");
    // Serial.println(b);
  }
}

void fadeToColor(int targetR, int targetG, int targetB, int fadeSpeed) {
  for (int i = 0; i <= 255; i++) {
    int r = R + (targetR - R) * i / 255;
    int g = G + (targetG - G) * i / 255;
    int b = B + (targetB - B) * i / 255;

    setColor(r, g, b);
    delay(fadeSpeed);
  }

  R = targetR;
  G = targetG;
  B = targetB;
}

void setColor(int red, int green, int blue) {
  R = red;
  G = green;
  B = blue;

  analogWrite(RED_PIN, red);
  analogWrite(GREEN_PIN, green);
  analogWrite(BLUE_PIN, blue);
}