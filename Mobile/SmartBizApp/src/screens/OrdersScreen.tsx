import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TextInput,
  TouchableOpacity,
  Switch,
  Alert,
  Modal,
  SafeAreaView,
  Platform,
  Dimensions,
  Animated,
  ActivityIndicator,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import { Picker } from '@react-native-picker/picker';
import ApiService from '../services/ApiService';
import { SaleOrder, CreateSaleOrderDto, Project, Company } from '../types';

const { width } = Dimensions.get('window');

const OrdersScreen: React.FC = () => {
  const [orders, setOrders] = useState<SaleOrder[]>([]);
  const [projects, setProjects] = useState<Project[]>([]);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [isDigitalMarketing, setIsDigitalMarketing] = useState(true);
  const [loading, setLoading] = useState(false);
  const [loadingOrders, setLoadingOrders] = useState(true);
  const [formData, setFormData] = useState<Partial<CreateSaleOrderDto>>({
    orderDate: new Date(),
    status: 'Draft',
    currencyCode: 'USD',
    customerAddress: '',
  });

  const slideAnim = new Animated.Value(0);

  useEffect(() => {
    loadOrders();
    loadProjects();
  }, []);

  const loadOrders = async () => {
    try {
      setLoadingOrders(true);
      const fetchedOrders = await ApiService.getSaleOrders();
      setOrders(fetchedOrders);
    } catch (error: any) {
      console.error('Error loading orders:', error);
      Alert.alert('Error', 'Failed to load orders: ' + error.message);
    } finally {
      setLoadingOrders(false);
    }
  };

  const loadProjects = async () => {
    try {
      const fetchedProjects = await ApiService.getProjects();
      setProjects(fetchedProjects);
    } catch (error: any) {
      console.error('Error loading projects:', error);
      // Don't show error for projects as it's not critical
    }
  };

  const determineBusinessType = async () => {
    try {
      // Try to get current user's company to determine business type
      const user = await ApiService.getCurrentUser();
      if (user?.company?.businessNature?.toLowerCase().includes('digital')) {
        setIsDigitalMarketing(true);
      } else {
        setIsDigitalMarketing(false);
      }
    } catch (error) {
      // Default to digital marketing if can't determine
      setIsDigitalMarketing(true);
    }
  };

  const openCreateForm = () => {
    Animated.spring(slideAnim, {
      toValue: 1,
      useNativeDriver: true,
    }).start();
    setShowCreateForm(true);
    determineBusinessType();
  };

  const closeCreateForm = () => {
    Animated.spring(slideAnim, {
      toValue: 0,
      useNativeDriver: true,
    }).start(() => {
      setShowCreateForm(false);
      setFormData({
        orderDate: new Date(),
        status: 'Draft',
        currencyCode: 'USD',
        customerAddress: '',
      });
    });
  };

  const handleSaveOrder = async () => {
    // Basic validation
    if (!formData.orderNumber || (!formData.clientName && !formData.customerName)) {
      Alert.alert('Validation Error', 'Please fill in all required fields');
      return;
    }

    setLoading(true);
    
    try {
      const orderData: CreateSaleOrderDto = {
        orderNumber: formData.orderNumber || '',
        orderDate: formData.orderDate || new Date(),
        customerName: isDigitalMarketing ? (formData.clientName || '') : (formData.customerName || ''),
        customerAddress: formData.customerAddress || '',
        items: [],
        
        // Digital Marketing specific fields
        ...(isDigitalMarketing && {
          clientName: formData.clientName,
          contactEmail: formData.contactEmail,
          contactPhone: formData.contactPhone,
          projectId: formData.projectId,
          deadline: formData.deadline,
          status: formData.status,
          orderDetails: formData.orderDetails,
          wordsOrPages: formData.wordsOrPages,
          academicLevel: formData.academicLevel,
          country: formData.country,
          assignedTo: formData.assignedTo,
          paymentStatus: formData.paymentStatus,
          sourceOfPayment: formData.sourceOfPayment,
          amountReceived: formData.amountReceived,
          amountReceivedInPkr: formData.amountReceivedInPkr,
          totalPayment: formData.totalPayment,
          currencyCode: formData.currencyCode,
          foreignCurrencyAmount: formData.foreignCurrencyAmount,
          pkrAmount: formData.pkrAmount,
          exchangeRateUsed: formData.exchangeRateUsed,
        }),
      };

      const newOrder = await ApiService.createSaleOrder(orderData);
      setOrders(prev => [newOrder, ...prev]);
      setLoading(false);
      closeCreateForm();
      Alert.alert('Success', 'Sale order created successfully!');
    } catch (error: any) {
      setLoading(false);
      Alert.alert('Error', 'Failed to create order: ' + error.message);
    }
  };

  const getStatusColor = (status: string = 'Draft') => {
    switch (status) {
      case 'Active': return '#28a745';
      case 'Completed': return '#007bff';
      case 'Cancelled': return '#dc3545';
      default: return '#ffc107';
    }
  };

  const formatDate = (date: Date | string): string => {
    if (!date) return 'N/A';
    const d = new Date(date);
    return d.toLocaleDateString();
  };

  const OrderCard = ({ order }: { order: SaleOrder }) => {
    const clientName = order.clientName || order.customerName || 'Unknown Client';
    const projectName = order.projectName || 'No Project';
    const isDigitalOrder = !!order.clientName;
    
    return (
      <View style={styles.orderCard}>
        <LinearGradient
          colors={['#ffffff', '#f8f9ff']}
          style={styles.cardGradient}
        >
          <View style={styles.cardHeader}>
            <View style={styles.orderInfo}>
              <Text style={styles.orderNumber}>{order.orderNumber}</Text>
              <Text style={styles.orderDate}>{formatDate(order.orderDate)}</Text>
              {order.deadline && (
                <Text style={styles.deadline}>Due: {formatDate(order.deadline)}</Text>
              )}
            </View>
            <View style={[styles.statusBadge, { backgroundColor: getStatusColor(order.status) }]}>
              <Text style={styles.statusText}>{order.status || 'Draft'}</Text>
            </View>
          </View>
          
          <View style={styles.cardBody}>
            <Text style={styles.clientName}>{clientName}</Text>
            <Text style={styles.projectName}>{projectName}</Text>
            {isDigitalOrder && <Text style={styles.businessType}>Digital Marketing</Text>}
            {order.contactEmail && (
              <Text style={styles.contactInfo}>📧 {order.contactEmail}</Text>
            )}
            {order.contactPhone && (
              <Text style={styles.contactInfo}>📱 {order.contactPhone}</Text>
            )}
            {order.academicLevel && (
              <Text style={styles.academicLevel}>🎓 {order.academicLevel}</Text>
            )}
            
            <View style={styles.cardFooter}>
              <View>
                {order.totalAmount > 0 && (
                  <Text style={styles.budget}>{order.currencyCode || 'USD'} {order.totalAmount}</Text>
                )}
                {order.foreignCurrencyAmount && (
                  <Text style={styles.foreignAmount}>
                    {order.currencyCode} {order.foreignCurrencyAmount}
                  </Text>
                )}
              </View>
              <TouchableOpacity style={styles.viewButton}>
                <Ionicons name="eye-outline" size={16} color="#667eea" />
                <Text style={styles.viewButtonText}>View</Text>
              </TouchableOpacity>
            </View>
          </View>
        </LinearGradient>
      </View>
    );
  };

  return (
    <SafeAreaView style={styles.container}>
      <LinearGradient
        colors={['#667eea', '#764ba2']}
        style={styles.headerGradient}
      >
        <View style={styles.header}>
          <Text style={styles.headerTitle}>Sale Orders</Text>
          <TouchableOpacity style={styles.addButton} onPress={openCreateForm}>
            <Ionicons name="add" size={24} color="white" />
          </TouchableOpacity>
        </View>
      </LinearGradient>

      <ScrollView style={styles.content} showsVerticalScrollIndicator={false}>
        {loadingOrders ? (
          <View style={styles.loadingContainer}>
            <ActivityIndicator size="large" color="#667eea" />
            <Text style={styles.loadingText}>Loading orders...</Text>
          </View>
        ) : orders.length === 0 ? (
          <View style={styles.emptyState}>
            <Ionicons name="receipt-outline" size={64} color="#ccc" />
            <Text style={styles.emptyTitle}>No Orders Yet</Text>
            <Text style={styles.emptySubtitle}>Create your first sale order to get started</Text>
          </View>
        ) : (
          orders.map((order) => (
            <OrderCard key={order.id} order={order} />
          ))
        )}
      </ScrollView>

      <Modal
        visible={showCreateForm}
        animationType="slide"
        presentationStyle="pageSheet"
      >
        <SafeAreaView style={styles.modalContainer}>
          <LinearGradient
            colors={['#667eea', '#764ba2']}
            style={styles.modalHeader}
          >
            <TouchableOpacity onPress={closeCreateForm} style={styles.closeButton}>
              <Ionicons name="close" size={24} color="white" />
            </TouchableOpacity>
            <Text style={styles.modalTitle}>Create Sale Order</Text>
            <View style={styles.placeholder} />
          </LinearGradient>

          <ScrollView style={styles.modalContent} showsVerticalScrollIndicator={false}>
            <View style={styles.formSection}>
              <Text style={styles.sectionTitle}>📋 Basic Information</Text>
              
              <View style={styles.inputGroup}>
                <Text style={styles.inputLabel}>Order Number *</Text>
                <TextInput
                  style={styles.input}
                  placeholder="Enter order number"
                  value={formData.orderNumber}
                  onChangeText={(text) => setFormData(prev => ({ ...prev, orderNumber: text }))}
                />
              </View>

              <View style={styles.inputGroup}>
                <Text style={styles.inputLabel}>Order Date *</Text>
                <TextInput
                  style={styles.input}
                  placeholder="YYYY-MM-DD"
                  value={formData.orderDate ? formatDate(formData.orderDate) : ''}
                  onChangeText={(text) => setFormData(prev => ({ ...prev, orderDate: new Date(text) }))}
                />
              </View>

              <View style={styles.switchGroup}>
                <Text style={styles.inputLabel}>Business Type</Text>
                <View style={styles.switchContainer}>
                  <Text style={styles.switchLabel}>Traditional</Text>
                  <Switch
                    value={isDigitalMarketing}
                    onValueChange={(value) => {
                      setIsDigitalMarketing(value);
                    }}
                    trackColor={{ false: '#ccc', true: '#667eea' }}
                    thumbColor={isDigitalMarketing ? '#764ba2' : '#f4f3f4'}
                  />
                  <Text style={styles.switchLabel}>Digital Marketing</Text>
                </View>
              </View>
            </View>

            <View style={styles.formSection}>
              <Text style={styles.sectionTitle}>👤 Client Information</Text>
              
              <View style={styles.inputGroup}>
                <Text style={styles.inputLabel}>
                  {isDigitalMarketing ? 'Client Name *' : 'Customer Name *'}
                </Text>
                <TextInput
                  style={styles.input}
                  placeholder={isDigitalMarketing ? "Enter client name" : "Enter customer name"}
                  value={isDigitalMarketing ? formData.clientName : formData.customerName}
                  onChangeText={(text) => setFormData(prev => ({ 
                    ...prev, 
                    [isDigitalMarketing ? 'clientName' : 'customerName']: text 
                  }))}
                />
              </View>

              {!isDigitalMarketing && (
                <View style={styles.inputGroup}>
                  <Text style={styles.inputLabel}>Customer Address</Text>
                  <TextInput
                    style={[styles.input, styles.textArea]}
                    placeholder="Enter customer address"
                    value={formData.customerAddress}
                    onChangeText={(text) => setFormData(prev => ({ ...prev, customerAddress: text }))}
                    multiline
                    numberOfLines={3}
                  />
                </View>
              )}

              <View style={styles.inputGroup}>
                <Text style={styles.inputLabel}>Contact Email</Text>
                <TextInput
                  style={styles.input}
                  placeholder="Enter contact email"
                  value={formData.contactEmail}
                  onChangeText={(text) => setFormData(prev => ({ ...prev, contactEmail: text }))}
                  keyboardType="email-address"
                  autoCapitalize="none"
                />
              </View>

              <View style={styles.inputGroup}>
                <Text style={styles.inputLabel}>Contact Phone</Text>
                <TextInput
                  style={styles.input}
                  placeholder="Enter contact phone"
                  value={formData.contactPhone}
                  onChangeText={(text) => setFormData(prev => ({ ...prev, contactPhone: text }))}
                  keyboardType="phone-pad"
                />
              </View>
            </View>

            {isDigitalMarketing && (
              <>
                <View style={styles.formSection}>
                  <Text style={styles.sectionTitle}>🚀 Project Information</Text>
                  
                  <View style={styles.inputGroup}>
                    <Text style={styles.inputLabel}>Project</Text>
                    <View style={styles.pickerContainer}>
                      <Picker
                        selectedValue={formData.projectId}
                        onValueChange={(itemValue) => setFormData(prev => ({ ...prev, projectId: itemValue }))}
                        style={styles.picker}
                      >
                        <Picker.Item label="Select Project" value={undefined} />
                        {projects.map((project) => (
                          <Picker.Item 
                            key={project.id} 
                            label={project.projectName} 
                            value={project.id} 
                          />
                        ))}
                      </Picker>
                    </View>
                  </View>

                  <View style={styles.inputGroup}>
                    <Text style={styles.inputLabel}>Deadline</Text>
                    <TextInput
                      style={styles.input}
                      placeholder="YYYY-MM-DD"
                      value={formData.deadline ? formatDate(formData.deadline) : ''}
                      onChangeText={(text) => setFormData(prev => ({ ...prev, deadline: new Date(text) }))}
                    />
                  </View>

                  <View style={styles.inputGroup}>
                    <Text style={styles.inputLabel}>Status</Text>
                    <View style={styles.pickerContainer}>
                      <Picker
                        selectedValue={formData.status}
                        onValueChange={(itemValue) => setFormData(prev => ({ ...prev, status: itemValue }))}
                        style={styles.picker}
                      >
                        <Picker.Item label="Draft" value="Draft" />
                        <Picker.Item label="Active" value="Active" />
                        <Picker.Item label="Completed" value="Completed" />
                        <Picker.Item label="Cancelled" value="Cancelled" />
                      </Picker>
                    </View>
                  </View>

                  <View style={styles.inputGroup}>
                    <Text style={styles.inputLabel}>Assigned To</Text>
                    <TextInput
                      style={styles.input}
                      placeholder="Enter assignee name"
                      value={formData.assignedTo}
                      onChangeText={(text) => setFormData(prev => ({ ...prev, assignedTo: text }))}
                    />
                  </View>

                  <View style={styles.inputGroup}>
                    <Text style={styles.inputLabel}>Order Details</Text>
                    <TextInput
                      style={[styles.input, styles.textArea]}
                      placeholder="Enter detailed requirements and specifications"
                      value={formData.orderDetails}
                      onChangeText={(text) => setFormData(prev => ({ ...prev, orderDetails: text }))}
                      multiline
                      numberOfLines={4}
                    />
                  </View>

                  <View style={styles.rowInputs}>
                    <View style={[styles.inputGroup, { flex: 1 }]}>
                      <Text style={styles.inputLabel}>Words/Pages</Text>
                      <TextInput
                        style={styles.input}
                        placeholder="e.g., 1000 words"
                        value={formData.wordsOrPages}
                        onChangeText={(text) => setFormData(prev => ({ ...prev, wordsOrPages: text }))}
                      />
                    </View>
                    
                    <View style={[styles.inputGroup, { flex: 1 }]}>
                      <Text style={styles.inputLabel}>Academic Level</Text>
                      <TextInput
                        style={styles.input}
                        placeholder="e.g., Bachelor"
                        value={formData.academicLevel}
                        onChangeText={(text) => setFormData(prev => ({ ...prev, academicLevel: text }))}
                      />
                    </View>
                  </View>

                  <View style={styles.inputGroup}>
                    <Text style={styles.inputLabel}>Country</Text>
                    <TextInput
                      style={styles.input}
                      placeholder="Enter country"
                      value={formData.country}
                      onChangeText={(text) => setFormData(prev => ({ ...prev, country: text }))}
                    />
                  </View>
                </View>

                <View style={styles.formSection}>
                  <Text style={styles.sectionTitle}>💰 Payment Information</Text>
                  
                  <View style={styles.inputGroup}>
                    <Text style={styles.inputLabel}>Payment Status</Text>
                    <View style={styles.pickerContainer}>
                      <Picker
                        selectedValue={formData.paymentStatus}
                        onValueChange={(itemValue) => setFormData(prev => ({ ...prev, paymentStatus: itemValue }))}
                        style={styles.picker}
                      >
                        <Picker.Item label="Pending" value="Pending" />
                        <Picker.Item label="Partial" value="Partial" />
                        <Picker.Item label="Paid" value="Paid" />
                        <Picker.Item label="Overdue" value="Overdue" />
                      </Picker>
                    </View>
                  </View>

                  <View style={styles.inputGroup}>
                    <Text style={styles.inputLabel}>Source of Payment</Text>
                    <TextInput
                      style={styles.input}
                      placeholder="e.g., Bank Transfer, PayPal"
                      value={formData.sourceOfPayment}
                      onChangeText={(text) => setFormData(prev => ({ ...prev, sourceOfPayment: text }))}
                    />
                  </View>

                  <View style={styles.rowInputs}>
                    <View style={[styles.inputGroup, { flex: 1 }]}>
                      <Text style={styles.inputLabel}>Currency</Text>
                      <TextInput
                        style={styles.input}
                        placeholder="USD"
                        value={formData.currencyCode}
                        onChangeText={(text) => setFormData(prev => ({ ...prev, currencyCode: text }))}
                      />
                    </View>
                    
                    <View style={[styles.inputGroup, { flex: 1 }]}>
                      <Text style={styles.inputLabel}>Exchange Rate</Text>
                      <TextInput
                        style={styles.input}
                        placeholder="0.00"
                        value={formData.exchangeRateUsed?.toString()}
                        onChangeText={(text) => setFormData(prev => ({ ...prev, exchangeRateUsed: parseFloat(text) || 0 }))}
                        keyboardType="numeric"
                      />
                    </View>
                  </View>

                  <View style={styles.rowInputs}>
                    <View style={[styles.inputGroup, { flex: 1 }]}>
                      <Text style={styles.inputLabel}>Foreign Amount</Text>
                      <TextInput
                        style={styles.input}
                        placeholder="0.00"
                        value={formData.foreignCurrencyAmount?.toString()}
                        onChangeText={(text) => setFormData(prev => ({ ...prev, foreignCurrencyAmount: parseFloat(text) || 0 }))}
                        keyboardType="numeric"
                      />
                    </View>
                    
                    <View style={[styles.inputGroup, { flex: 1 }]}>
                      <Text style={styles.inputLabel}>PKR Amount</Text>
                      <TextInput
                        style={styles.input}
                        placeholder="0.00"
                        value={formData.pkrAmount?.toString()}
                        onChangeText={(text) => setFormData(prev => ({ ...prev, pkrAmount: parseFloat(text) || 0 }))}
                        keyboardType="numeric"
                      />
                    </View>
                  </View>

                  <View style={styles.rowInputs}>
                    <View style={[styles.inputGroup, { flex: 1 }]}>
                      <Text style={styles.inputLabel}>Amount Received</Text>
                      <TextInput
                        style={styles.input}
                        placeholder="0.00"
                        value={formData.amountReceived?.toString()}
                        onChangeText={(text) => setFormData(prev => ({ ...prev, amountReceived: parseFloat(text) || 0 }))}
                        keyboardType="numeric"
                      />
                    </View>
                    
                    <View style={[styles.inputGroup, { flex: 1 }]}>
                      <Text style={styles.inputLabel}>Received (PKR)</Text>
                      <TextInput
                        style={styles.input}
                        placeholder="0.00"
                        value={formData.amountReceivedInPkr?.toString()}
                        onChangeText={(text) => setFormData(prev => ({ ...prev, amountReceivedInPkr: parseFloat(text) || 0 }))}
                        keyboardType="numeric"
                      />
                    </View>
                  </View>

                  <View style={styles.inputGroup}>
                    <Text style={styles.inputLabel}>Total Payment</Text>
                    <TextInput
                      style={styles.input}
                      placeholder="0.00"
                      value={formData.totalPayment?.toString()}
                      onChangeText={(text) => setFormData(prev => ({ ...prev, totalPayment: parseFloat(text) || 0 }))}
                      keyboardType="numeric"
                    />
                  </View>
                </View>
              </>
            )}

            <TouchableOpacity 
              style={styles.saveButton} 
              onPress={handleSaveOrder}
              disabled={loading}
            >
              <LinearGradient
                colors={['#667eea', '#764ba2']}
                style={styles.saveButtonGradient}
              >
                {loading ? (
                  <ActivityIndicator color="white" />
                ) : (
                  <>
                    <Ionicons name="checkmark" size={20} color="white" />
                    <Text style={styles.saveButtonText}>Create Order</Text>
                  </>
                )}
              </LinearGradient>
            </TouchableOpacity>

            <View style={{ height: 100 }} />
          </ScrollView>
        </SafeAreaView>
      </Modal>
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f8f9fa',
  },
  headerGradient: {
    paddingTop: Platform.OS === 'ios' ? 0 : 25,
  },
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: 20,
    paddingVertical: 15,
  },
  headerTitle: {
    fontSize: 24,
    fontWeight: 'bold',
    color: 'white',
  },
  addButton: {
    backgroundColor: 'rgba(255, 255, 255, 0.2)',
    borderRadius: 20,
    padding: 8,
  },
  content: {
    flex: 1,
    paddingHorizontal: 20,
    paddingTop: 20,
  },
  orderCard: {
    marginBottom: 15,
    borderRadius: 20,
    overflow: 'hidden',
    shadowColor: '#000',
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.1,
    shadowRadius: 8,
    elevation: 5,
  },
  cardGradient: {
    padding: 20,
  },
  cardHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginBottom: 15,
  },
  orderInfo: {
    flex: 1,
  },
  orderNumber: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 4,
  },
  orderDate: {
    fontSize: 14,
    color: '#666',
  },
  statusBadge: {
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 20,
  },
  statusText: {
    color: 'white',
    fontSize: 12,
    fontWeight: 'bold',
  },
  cardBody: {
    marginBottom: 15,
  },
  clientName: {
    fontSize: 16,
    fontWeight: '600',
    color: '#333',
    marginBottom: 4,
  },
  projectName: {
    fontSize: 14,
    color: '#667eea',
    marginBottom: 4,
  },
  businessType: {
    fontSize: 12,
    color: '#764ba2',
    marginBottom: 4,
  },
  deadline: {
    fontSize: 12,
    color: '#ff6b6b',
    fontWeight: '500',
  },
  contactInfo: {
    fontSize: 12,
    color: '#666',
    marginBottom: 2,
  },
  academicLevel: {
    fontSize: 12,
    color: '#28a745',
    marginBottom: 8,
  },
  cardFooter: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  budget: {
    fontSize: 16,
    fontWeight: 'bold',
    color: '#28a745',
  },
  foreignAmount: {
    fontSize: 12,
    color: '#666',
  },
  loadingContainer: {
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 80,
  },
  loadingText: {
    marginTop: 10,
    fontSize: 16,
    color: '#666',
  },
  viewButton: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: 'rgba(102, 126, 234, 0.1)',
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 15,
  },
  viewButtonText: {
    color: '#667eea',
    fontSize: 12,
    fontWeight: '600',
    marginLeft: 4,
  },
  emptyState: {
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 80,
  },
  emptyTitle: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#333',
    marginTop: 20,
    marginBottom: 8,
  },
  emptySubtitle: {
    fontSize: 14,
    color: '#666',
    textAlign: 'center',
    paddingHorizontal: 40,
  },
  modalContainer: {
    flex: 1,
    backgroundColor: '#f8f9fa',
  },
  modalHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: 20,
    paddingVertical: 15,
  },
  closeButton: {
    backgroundColor: 'rgba(255, 255, 255, 0.2)',
    borderRadius: 20,
    padding: 8,
  },
  modalTitle: {
    fontSize: 20,
    fontWeight: 'bold',
    color: 'white',
  },
  placeholder: {
    width: 40,
  },
  modalContent: {
    flex: 1,
    paddingHorizontal: 20,
  },
  formSection: {
    backgroundColor: 'white',
    borderRadius: 20,
    padding: 20,
    marginTop: 20,
    shadowColor: '#000',
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 20,
  },
  inputGroup: {
    marginBottom: 20,
  },
  inputLabel: {
    fontSize: 16,
    fontWeight: '600',
    color: '#333',
    marginBottom: 8,
  },
  input: {
    borderWidth: 2,
    borderColor: '#e0e0e0',
    borderRadius: 12,
    paddingHorizontal: 16,
    paddingVertical: 12,
    fontSize: 16,
    backgroundColor: '#f8f9ff',
  },
  textArea: {
    height: 100,
    textAlignVertical: 'top',
  },
  switchGroup: {
    marginBottom: 20,
  },
  switchContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    backgroundColor: '#f0f0f0',
    borderRadius: 25,
    paddingHorizontal: 20,
    paddingVertical: 10,
  },
  switchLabel: {
    fontSize: 14,
    fontWeight: '600',
    color: '#333',
  },
  rowInputs: {
    flexDirection: 'row',
    gap: 15,
  },
  pickerContainer: {
    borderWidth: 2,
    borderColor: '#e0e0e0',
    borderRadius: 12,
    backgroundColor: '#f8f9ff',
  },
  picker: {
    height: 50,
  },
  saveButton: {
    marginTop: 30,
    marginHorizontal: 20,
    borderRadius: 25,
    overflow: 'hidden',
  },
  saveButtonGradient: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 15,
    paddingHorizontal: 30,
  },
  saveButtonText: {
    color: 'white',
    fontSize: 18,
    fontWeight: 'bold',
    marginLeft: 8,
  },
});

export default OrdersScreen;