---@meta

---@class mathf
---@field pi number
---@field tau number
---@field e number
mathf = {};

-- helper functions

---@param d number
---@return number
---@nodiscard
function mathf:abs(d) end

---@param value number
---@param min number
---@param max number
---@return number
---@nodiscard
function mathf:clamp(value, min, max) end

---@param a number
---@param b number
---@return number
---@nodiscard
function mathf:min(a, b) end

---@param a number
---@param b number
---@return number
---@nodiscard
function mathf:max(a, b) end

---@param d number
---@return integer
---@nodiscard
function mathf:sign(d) end

---@param d number
---@return number
---@nodiscard
function mathf:floor(d) end

---@param d number
---@param threshold number
---@return number
---@nodiscard
function mathf:floort(d, threshold) end

---@param d number
---@return number
---@nodiscard
function mathf:ceil(d) end

---@param d number
---@param threshold number
---@return number
---@nodiscard
function mathf:ceilt(d, threshold) end

---@param d number
---@return number
---@nodiscard
function mathf:round(d) end

---@param d number
---@param threshold number
---@return number
---@nodiscard
function mathf:roundt(d, threshold) end

---@param d number
---@return number
---@nodiscard
function mathf:sqrt(d) end

---@param x number
---@param y number
---@return number
---@nodiscard
function mathf:pow(x, y) end

---@param d number
---@return number
---@nodiscard
function mathf:exp(d) end

---natural log (ln)
---@param d number
---@return number
---@nodiscard
function mathf:log(d) end

---@param d number
---@return number
---@nodiscard
function mathf:log10(d) end

---@param a number
---@param b number
---@param t number
---@return number
---@nodiscard
function mathf:lerp(a, b, t) end

-- trigonometric functions

---@param d number
---@return number
---@nodiscard
function mathf:sin(d) end

---@param d number
---@return number
---@nodiscard
function mathf:cos(d) end

---@param d number
---@return number
---@nodiscard
function mathf:tan(d) end

---@param d number
---@return number
---@nodiscard
function mathf:asin(d) end

---@param d number
---@return number
---@nodiscard
function mathf:acos(d) end

---@param d number
---@return number
---@nodiscard
function mathf:atan(d) end

---@param x number
---@param y number
---@return number
---@nodiscard
function mathf:atan2(x, y) end

---@param radians number
---@return number
---@nodiscard
function mathf:deg(radians) end

---@param degrees number
---@return number
---@nodiscard
function mathf:rad(degrees) end

---@param d number
---@return number
---@nodiscard
function mathf:sinh(d) end

---@param d number
---@return number
---@nodiscard
function mathf:cosh(d) end

---@param d number
---@return number
---@nodiscard
function mathf:tanh(d) end

---@param a Vector2
---@param b Vector2
---@return Vector2
---@nodiscard
function mathf:vecsub(a, b) end

---@param a Vector2
---@param b Vector2
---@return Vector2
---@nodiscard
function mathf:vecadd(a, b) end

---multiplies a vector by a scalar value
---@param a Vector2
---@param scalar number
---@return Vector2
---@nodiscard
function mathf:vecmul(a, scalar) end

---@param a Vector2
---@param b Vector2
---@return number
---@nodiscard
function mathf:vecdot(a, b) end

---gets the magnitude of the vector
---@param v Vector2
---@return number
---@nodiscard
function mathf:veclen(v) end

---@param v Vector2
---@return Vector2
---@nodiscard
function mathf:vecnorm(v) end

---rotates a vector by an angle around the origin
---@param v Vector2 the vector to rotate
---@param angle number Angle in radians
---@return Vector2
---@nodiscard
function mathf:vecrotate(v, angle) end

---rotates vector a around vector b by an angle
---@param a Vector2 the vector to rotate
---@param b Vector2 origin/pivot
---@param angle number Angle in radians
---@return Vector2
---@nodiscard
function mathf:vecrotatearound(a, b, angle) end

---@param v Vector2 
---@return Vector2
---@nodiscard
function mathf:vecabs(v) end

---@param a Vector2
---@param b Vector2
---@param acceptableDifference number (optional)
---@return boolean
---@nodiscard
function mathf:vecequals(a, b, acceptableDifference) end

---gets angle of vector in radians
---@param v Vector2 The vector
---@return number Angle in radians
---@nodiscard
function mathf:vecangle(v) end

---gets the angle between two vectors in radians
---@param a Vector2
---@param b Vector2
---@return number Angle in radians
---@nodiscard
function mathf:vecanglebetween(a, b) end

---gets the distance between two points/vectors
---@param a Vector2
---@param b Vector2
---@return number
---@nodiscard
function mathf:vecdist(a, b) end

---@param v Vector2
---@param acceptableDifference number (optional)
---@return boolean
---@nodiscard
function mathf:vecisnorm(v, acceptableDifference) end

---@param v Vector2
---@param acceptableDifference number (optional)
---@return boolean
---@nodiscard
function mathf:veciszero(v, acceptableDifference) end

return mathf