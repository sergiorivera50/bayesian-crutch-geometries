import * as THREE from 'three';
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';

const scene = new THREE.Scene();
scene.background = new THREE.Color(0x000000); // Set to black or another contrasting color
const camera = new THREE.PerspectiveCamera(32, window.innerWidth / window.innerHeight, 0.1, 1000);
const renderer = new THREE.WebGLRenderer();
renderer.setSize(window.innerWidth, window.innerHeight);
document.body.appendChild(renderer.domElement);

const controls = new OrbitControls(camera, renderer.domElement);
controls.enableDamping = true; // Optional, but this gives a smoother control feeling
controls.dampingFactor = 0.1;
controls.rotateSpeed = 0.2;

const cubeSize = 1.5;  // Dimension of the cube (2 units wide)
const cubeGeometry = new THREE.BoxGeometry(cubeSize, cubeSize, cubeSize);
const cubeMaterial = new THREE.MeshBasicMaterial({ color: 0x00ff00, wireframe: true });
const cube = new THREE.Mesh(cubeGeometry, cubeMaterial);
scene.add(cube);

fetch('./experiments.json')
  .then(response => response.json())
  .then(data => {
    display(data);
  })
  .catch(error => console.error('Error loading JSON:', error));

function display(data) {
  function getMax(data, index) {
    return Math.max(...data.map(item => item[index]));
  }

  function normalizeData(value, max) {
    return (value / max * cubeSize) - (cubeSize / 2);
  }

  // Finding maximum values for each dimension with a 10% buffer
  const x1Max = getMax(data.map(item => item.params), 0) * 1.1;
  const x2Max = getMax(data.map(item => item.params), 1) * 1.1;
  const x3Max = getMax(data.map(item => item.params), 2) * 1.1;

  // Normalizing the data
  const normalizedData = data.map(item => {
    return {
      ...item,
      position: [
        normalizeData(item.params[0], x1Max),
        normalizeData(item.params[1], x2Max),
        normalizeData(item.params[2], x3Max)
      ],
    };
  });

  console.log(normalizedData)

  // Normalize for color mapping

  const yMin = Math.min(...data.map(item => item.loss));
  const yMax = Math.max(...data.map(item => item.loss));

  function normalize(value, min, max) {
    return (value - min) / (max - min);
  }

  function getColor(value) {
    const normalizedValue = normalize(value, yMin, yMax);
    const lightness = 100 - normalizedValue * 50; // Lightness varies from 50% to 100%
    return `hsl(0, 100%, ${lightness}%)`; // Fixed hue (red), 100% saturation
  }

  const sphereRadius = 0.02; // Adjust the size of the spheres

  const spheres = new THREE.Group(); // Group to hold all spheres

  // Add lights to the scene
  const ambientLight = new THREE.AmbientLight(0xffffff, 0.5); // soft white light
  scene.add(ambientLight);

  const directionalLight = new THREE.DirectionalLight(0xffffff, 1);
  directionalLight.position.set(0, 1, 1);
  scene.add(directionalLight);

  // Use MeshPhongMaterial for the spheres
  const sphereMaterial = new THREE.MeshPhongMaterial();

  normalizedData.forEach((item, index) => {
    const sphereGeometry = new THREE.SphereGeometry(sphereRadius, 32, 32);
    const sphere = new THREE.Mesh(sphereGeometry, sphereMaterial.clone());

    // Ensure the getColor function returns an RGB string
    sphere.material.color = new THREE.Color(getColor(item.loss));
    sphere.position.set(item.position[0], item.position[1], item.position[2]);
    sphere.userData = { item };

    spheres.add(sphere);
  })

  scene.add(spheres);

  camera.position.z = 5;

  const mouse = new THREE.Vector2();
  const raycaster = new THREE.Raycaster();
  const tooltip = document.getElementById('tooltip');

  function onMouseMove(event) {
    // Calculate mouse position in normalized device coordinates (-1 to +1) for both components
    mouse.x = (event.clientX / window.innerWidth) * 2 - 1;
    mouse.y = - (event.clientY / window.innerHeight) * 2 + 1;
  }

  function showTooltip(intersects) {
    if (intersects.length > 0) {
      const item = intersects[0].object.userData.item;
      tooltip.innerHTML = `${item.name}: [${item.params}] -> ${item.loss}`;
      tooltip.style.display = 'block';
      tooltip.style.left = (event.clientX + 10) + 'px';
      tooltip.style.top = (event.clientY + 10) + 'px';
    } else {
      tooltip.style.display = 'none';
    }
  }

  function animate() {
    requestAnimationFrame(animate);

    raycaster.setFromCamera(mouse, camera);
    const intersects = raycaster.intersectObjects(spheres.children); // Use intersectObjects
    showTooltip(intersects);

    controls.update();
    renderer.render(scene, camera);
  }

  window.addEventListener('mousemove', onMouseMove, false);

  window.addEventListener('resize', () => {
    renderer.setSize(window.innerWidth, window.innerHeight);
    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix();
  });

  animate();
}
